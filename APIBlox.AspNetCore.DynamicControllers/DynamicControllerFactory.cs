using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Exceptions;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;

namespace APIBlox.AspNetCore
{
    /// <summary>
    ///     Class DynamicControllerTemplateExtensions.
    /// </summary>
    public class DynamicControllerFactory
    {
        // Found the following piece of gold at...
        // https://github.com/dotnet/roslyn/wiki/Runtime-code-generation-using-Roslyn-compilations-in-.NET-Core-App
        private static readonly PortableExecutableReference[] References = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            .ToString()
            .Split(";", StringSplitOptions.RemoveEmptyEntries)
            .Select(s => MetadataReference.CreateFromFile(s))
            .ToArray();

        private readonly string _assemblyName;
        private readonly ILogger<DynamicControllerFactory> _log;
        private readonly bool _release;
        private readonly bool _addControllerComments;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicControllerFactory" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="release">if set to <c>true</c> [release configuration].</param>
        /// <param name="addControllerComments">
        ///     if set to <c>true</c> Summary comments from the request object of the first
        ///     controller action added will be included as controller summary comments.
        /// </param>
        /// <exception cref="ArgumentNullException">assemblyName</exception>
        public DynamicControllerFactory(ILoggerFactory loggerFactory, string assemblyName, bool release = false, bool addControllerComments = true)
        {
            if (assemblyName.IsEmptyNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(assemblyName));

            _assemblyName = assemblyName;
            _release = release;
            _addControllerComments = addControllerComments;
            _log = loggerFactory.CreateLogger<DynamicControllerFactory>();
        }

        /// <summary>
        ///     A collection of additional assemblies they may be required when compiling.
        /// </summary>
        /// <value>The additional assembly references.</value>
        public List<Assembly> AdditionalAssemblyReferences { get; } = new List<Assembly>();

        /// <summary>
        ///     Gets the compilation errors.
        /// </summary>
        /// <value>The compilation errors.</value>
        public IEnumerable<string> Errors { get; private set; }

        /// <summary>
        ///     Gets the compilation warnings.
        /// </summary>
        /// <value>The compilation warnings.</value>
        public IEnumerable<string> Warnings { get; private set; }

        /// <summary>
        ///     Gets the controllers used during compilation.
        /// </summary>
        /// <value>The controllers.</value>
        public IDictionary<string, string> Controllers { get; private set; }

        /// <summary>
        ///     Gets the output files when compiling with an assembly output path.
        /// </summary>
        /// <value>The output files.</value>
        public (string, string, string, string[]) OutputFiles { get; private set; }

        /// <summary>
        ///     Compiles 1 or more <see cref="IComposedTemplate" /> to the specified assemblyOutputPath.
        ///     <para>
        ///         When null is returned, then errors have been generated, check the <see cref="Errors" /> property.
        ///     </para>
        ///     <para>
        ///         When useCache is true: If assembly, pdb (if not production) and xml file(s)
        ///         already exist, then they will be returned without compiling anything.
        ///     </para>
        /// </summary>
        /// <param name="assemblyOutputPath">The assembly output path.</param>
        /// <param name="templates">The templates.</param>
        /// <returns>Assembly.</returns>
        /// <exception cref="ArgumentNullException">assemblyOutputPath</exception>
        public Assembly Compile(string assemblyOutputPath, params IComposedTemplate[] templates)
        {
            if (assemblyOutputPath.IsEmptyNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(assemblyOutputPath));

            if (!Directory.Exists(assemblyOutputPath))
                Directory.CreateDirectory(assemblyOutputPath);

            var fi = EmitToFile(assemblyOutputPath, templates);

            if (!(Errors is null))
                return null;

            return !(fi is null) && fi.Exists ? Assembly.LoadFile(fi.FullName) : null;
        }

        ///// <summary>
        /////     Compiles 1 or more <see cref="IComposedTemplate" /> to an assembly in memory.
        /////     <para>
        /////         When null is returned, then errors have been generated, check the <see cref="Errors" /> property.
        /////     </para>
        ///// </summary>
        ///// <param name="templates">The templates.</param>
        ///// <returns>IEnumerable&lt;Type&gt;.</returns>
        //public Assembly Compile(params IComposedTemplate[] templates)
        //{
        //    return EmitToAssembly(templates);
        //}

        /// <summary>
        ///     Given a type this will generate method input parameters in string form along with namespaces.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.ValueTuple&lt;System.String, System.String[]&gt;.</returns>
        public static (string, string[]) WriteInputParamsWithNamespaces(Type obj)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.codedom.compiler.codedomprovider?view=netframework-4.7.2

            var namespaces = new List<string> { obj.Namespace };
            var method = new CodeMemberMethod { Name = "DummyMethod" };

            var properties = GetPublicReadWriteProperties(obj);

            foreach (var pi in properties)
            {
                namespaces.Add(pi.PropertyType.Namespace);

                var attributes = pi.GetCustomAttributes();

                var arg = new CodeParameterDeclarationExpression(pi.PropertyType, pi.Name.ToCamelCase());

                foreach (var att in attributes)
                {
                    var attType = att.GetType();

                    namespaces.Add(attType.Namespace);

                    // Create the attribute declaration for the property.
                    var attr = new CodeAttributeDeclaration(new CodeTypeReference(attType));

                    attr.Arguments.AddRange(GetConstructorCodeArguments(namespaces, att).ToArray());
                    attr.Arguments.AddRange(GetPropertyCodeArguments(namespaces, att).ToArray());

                    arg.CustomAttributes.Add(attr);
                }

                method.Parameters.Add(arg);
            }

            namespaces = namespaces.Distinct().ToList();
            var tmpNs = namespaces.OrderByDescending(s => s.Length).Select(s => $"{s}.").ToList();

            var code = WriteMethod(method);

            var ret = tmpNs.Aggregate(code, (cur, ns) => cur.Replace(ns, "")).Trim();

            // TOD: find regex for this.
            var first = ret.IndexOfEx("(") + 1;
            var last = ret.LastIndexOfEx(")");
            var result = ret.Substring(first, last - first)
                .Replace("()", "");

            return (result, namespaces.ToArray());
        }

        /// <summary>
        ///     Gets the input parameters XML comments if they exist.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static IEnumerable<string> WriteInputParamsXmlComments(Type obj)
        {
            const string template = "@space/// <param name =\"@pName\">@pComment</param>";
            var props = GetPublicReadWriteProperties(obj);

            var parameters = new List<string>();

            for (var index = 0; index < props.Count; index++)
            {
                var pi = props[index];

                try
                {
                    var xml = pi.GetSummary();

                    var space = index == 0 ? "" : "        ";

                    parameters.Add(template
                        .Replace("@space", space)
                        .Replace("@pName", pi.Name.ToCamelCase())
                        .Replace("@pComment", xml)
                    );
                }
                catch (Exception ex)
                {
                    throw new Exception($"Could not get xml summary for type {obj.Name}.", ex);
                }
            }

            return parameters;
        }

        /// <summary>
        ///     Given a type this will get a types name and namespaces.  If a generic type, then the first argument is used as the
        ///     type name.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.ValueTuple&lt;System.String, System.String, System.String[]&gt;.</returns>
        public static (string, string, string[]) WriteNameWithNamespaces(Type obj)
        {
            var name = GetNameWithoutGenericArity(obj);
            var ns = obj.Namespace;
            var namespaces = new List<string> { ns };

            var result = new StringBuilder();
            result.Append($"{name}");

            if (!obj.IsGenericType)
                return (result.ToString(), null, namespaces.ToArray());

            result.Append("<");

            var args = obj.GetGenericArguments();

            for (var index = 0; index < args.Length; index++)
            {
                var type = args[index];
                var tNs = type.Namespace;
                var tName = type.Name;

                result.Append(index == args.Length - 1 ? $"{tName}" : $"{tName},");

                if (!namespaces.Contains(tNs))
                    namespaces.Add(tNs);
            }

            result.Append(">");

            return (result.ToString(), args[0].Name, namespaces.ToArray());
        }

        /// <summary>
        ///     To the new object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.String.</returns>
        public static string WriteNewObject(Type obj)
        {
            var props = GetPublicReadWriteProperties(obj);

            var setters = new StringBuilder();

            for (var index = 0; index < props.Count; index++)
            {
                var pi = props[index];
                var comma = index == props.Count - 1 ? "" : ", ";
                setters.Append($"{pi.Name} = {pi.Name.ToCamelCase()}{comma}");
            }

            return $"new {obj.Name}{{{setters}}}";
        }

        /// <summary>
        ///     Validates the type of the response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValidateResponseType(Type response)
        {
            if (response.IsPublic && !response.IsAbstract)// && response.GetConstructors(BindingFlags.Public | BindingFlags.Instance) != null)
                return;

            if (!response.IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException($"{response.Name} must be public, non abstract and have a public parameter-less constructor.");
        }

        /// <summary>
        ///     Validates the type of the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="mustHaveBodyProperty">if set to <c>true</c> [must have body property].</param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static void ValidateRequestType(Type request, bool mustHaveBodyProperty = false)
        {
            var readProps = request.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead)
                .ToList();

            if (!readProps.Any())
                throw new ArgumentException($"{request.Name} must have at least 1 publicly accessible getter property.");

            if (!mustHaveBodyProperty)
                return;

            var count = readProps.Count(pi => pi.GetCustomAttribute<FromBodyAttribute>() != null);

            if (count != 1)
                throw new ArgumentException($"{request.Name} must have a public property that is decorated with a {nameof(FromBodyAttribute)}.");
        }

        private static void AddExistingArray(List<string> dest, IEnumerable<string> src)
        {
            if (src is null)
                return;

            dest.AddRange(src);
        }

        //private Assembly EmitToAssembly(params IComposedTemplate[] templates)
        //{
        //    Reset();

        //    var csOptions = GetSyntaxTree(templates, out var csSyntaxTree, out _);

        //    var compilation = CSharpCompilation.Create(_assemblyName, csSyntaxTree, GetReferences(), csOptions);

        //    using (var ms = new MemoryStream())
        //    {
        //        var emitResult = compilation.Emit(ms);

        //        if (emitResult.Success)
        //        {
        //            CheckAndSetWarnings(emitResult);

        //            ms.Seek(0, SeekOrigin.Begin);

        //            var assembly = Assembly.Load(ms.ToArray());

        //            _log.LogInformation(() => $"Created dynamic controllers assembly: {assembly.FullName}");

        //            return assembly;
        //        }

        //        _log.LogCritical(() => $"Could not create dynamic controllers assembly: {_assemblyName}");

        //        CheckAndSetFailures(emitResult);

        //        return null;
        //    }
        //}

        private FileInfo EmitToFile(string outputFolder, params IComposedTemplate[] templates)
        {
            Reset();

            var csOptions = GetSyntaxTree(outputFolder, templates, out var csSyntaxTree, out var csFiles);

            var compilation = CSharpCompilation.Create(_assemblyName, csSyntaxTree, GetReferences(), csOptions);
            var dllFile = Path.Combine(outputFolder, $"{_assemblyName}.dll");
            var pdbFile = Path.Combine(outputFolder, $"{_assemblyName}.pdb");
            var xmlFile = Path.Combine(outputFolder, $"{_assemblyName}.xml");

            try
            {
                var emitResult = compilation.Emit(dllFile, pdbFile, xmlFile);

                if (emitResult.Success)
                {
                    var dll = new FileInfo(dllFile);
                    var pdb = new FileInfo(pdbFile);
                    var xml = new FileInfo(xmlFile);

                    OutputFiles = (dll.FullName, pdb.FullName, xml.FullName, csFiles);

                    _log.LogInformation(() => $"Created dynamic controllers assembly file: {dll.FullName}");

                    CheckAndSetWarnings(emitResult);

                    return dll;
                }

                _log.LogCritical(() => $"Could not create dynamic controllers assembly file: {dllFile}");

                CheckAndSetFailures(emitResult);
            }
            catch (IOException ioEx)
            {
                if (File.Exists(dllFile))
                {
                    Warnings = new List<string> { ioEx.Message };
                    _log.LogWarning(() => $"Could not create dynamic controllers assembly file: {dllFile}.  Its in use!");
                }
                else
                {
                    Errors = new List<string> { ioEx.Message };
                    _log.LogCritical(() => $"Could not create dynamic controllers assembly file: {dllFile}.  Ex: {ioEx.Message}");
                }

                return null;
            }

            return null;
        }

        private IEnumerable<MetadataReference> GetReferences()
        {
            if (!AdditionalAssemblyReferences.Any())
                return References;

            var lst = new List<PortableExecutableReference>(References);

            foreach (var assembly in AdditionalAssemblyReferences)
            {
                if (References.Any(p => p.FilePath.EqualsEx(assembly.Location)))
                    continue;

                using var ass = new AssemblyResolver();

                ass.LoadFromAssemblyPath(assembly.Location, out _);

                lst.AddRange(ass.LoadedReferencedAssemblies
                    .Where(an => File.Exists(an.Location) && References.All(p => p.FilePath != an.Location))
                    .Select(a => MetadataReference.CreateFromFile(a.Location))
                );
            }

            return lst;
        }

        private void Reset()
        {
            OutputFiles = (null, null, null, null);
            Errors = null;
            Warnings = null;
            Controllers = null;
        }

        private CSharpCompilationOptions GetSyntaxTree(string outputFolder, IComposedTemplate[] templates,
            out IEnumerable<SyntaxTree> csSyntaxTree, out string[] csFiles)
        {
            if (templates is null || !templates.Any())
                throw new ArgumentNullException(nameof(templates));

            var csOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: _release
                    ? OptimizationLevel.Release
                    : OptimizationLevel.Debug
            );

            // merge templates, make sure same route for controller name.
            var controllerGroups = templates.GroupBy(g => g.Name)
                .Select(g => g)
                .ToList();

            var results = new Dictionary<string, string>();

            foreach (var cg in controllerGroups)
            {
                if (cg.GroupBy(r => r.Route).Count() > 1 || cg.GroupBy(r => r.Namespace).Count() > 1)
                    throw new ArgumentException(
                        $"Controller {cg.Key} has more " +
                        "than one route or namespace specified. Name, Route and Namespace must " +
                        "be identical (if your intention is that this is ONE controller)",
                        nameof(IComposedTemplate.Route)
                    );

                var first = cg.First();
                var dc = new DynamicController(first.Name,
                    first.Namespace,
                    first.Route,
                    _addControllerComments ? first.Comments : ""
                );

                foreach (var da in cg)
                {
                    AddExistingArray(dc.Namespaces, da.Action.Namespaces);
                    AddExistingArray(dc.Fields, da.Action.Fields);

                    if (!da.Action.Methods.IsEmptyNullOrWhiteSpace())
                        dc.Methods.Add(da.Action.Methods);

                    dc.CtorArgs.Add(da.Action.CtorArgs);
                    dc.CtorBody.Add(da.Action.CtorBody);
                    dc.Actions.Add(da.Action.Content);
                }

                var result = CSharpSyntaxTree.ParseText(dc.ToString()).GetRoot().NormalizeWhitespace().ToFullString();

                results.Add(cg.Key, result);
            }

            var defEncoding = Encoding.UTF8;

            var files = new List<string>();
            Controllers = results;
            csSyntaxTree = results.Select(r =>
            {
                var (key, code) = r;
                var srcPath = $"{key}.cs";
                var fullPath = Path.Combine(outputFolder, srcPath);
                var tree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(), srcPath);
                var syntaxRootNode = tree.GetRoot() as CSharpSyntaxNode;
                var encoded = CSharpSyntaxTree.Create(syntaxRootNode, null, srcPath, defEncoding);

                if (_release)
                    return encoded;

                File.WriteAllText(fullPath, code, defEncoding);

                files.Add(fullPath);

                return encoded;
            }).ToList();

            csFiles = files.Any() ? files.ToArray() : null;
            return csOptions;
        }

        private void CheckAndSetFailures(EmitResult emitResult)
        {
            var diagnostics = emitResult.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error
            );

            var failures = diagnostics.Select(diagnostic =>
                $"{diagnostic.Id}: {diagnostic.GetMessage()}"
            ).ToArray();

            Errors = failures.Any() ? failures : null;
        }

        private void CheckAndSetWarnings(EmitResult emitResult)
        {
            var diagnostics = emitResult.Diagnostics.Where(diagnostic =>
                !diagnostic.IsSuppressed
            );

            var warnings = diagnostics.Select(diagnostic =>
                $"{diagnostic.Id}: {diagnostic.GetMessage()}"
            ).ToArray();

            Warnings = warnings.Any() ? warnings : null;
        }

        private static string WriteMethod(CodeTypeMember method)
        {
            // https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-create-a-class-using-codedom
            using (var provider = CodeDomProvider.CreateProvider("CSharp"))
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var options = new CodeGeneratorOptions { BracingStyle = "C" };

                            provider.GenerateCodeFromMember(method, writer, options);

                            writer.Flush();
                            stream.Position = 0;

                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private static IEnumerable<CodeAttributeArgument> GetConstructorCodeArguments(ICollection<string> namespaces, object obj)
        {
            var type = obj.GetType();

            namespaces.Add(type.Namespace);

            var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();

            if (ctor is null)
                yield break;

            var ctorArgs = ctor.GetParameters().ToList();

            if (!ctorArgs.Any())
                yield break;

            var readProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetValue(obj) != default)
                .ToList();

            foreach (var cp in ctorArgs)
            {
                var cp1 = cp;
                var cpi = readProps.FirstOrDefault(p => p.Name.EqualsEx(cp1.Name));

                if (cpi is null)
                    throw new TemplateCompilationException(
                        new[] { $"Attribute {type.Name} does not have a GETTER, parser can NOT get current values for constructor!" }
                    );

                namespaces.Add(cp.ParameterType.Namespace);

                var value = cpi.GetValue(obj);
                var valType = value.GetType();

                if (valType.IsPrimitive || valType == typeof(string))
                {
                    namespaces.Add(valType.Namespace);

                    yield return new CodeAttributeArgument(new CodePrimitiveExpression(value));
                }
                else if (value is IEnumerable collection)
                {
                    namespaces.Add(collection.GetType().Namespace);

                    foreach (var itm in collection)
                    {
                        namespaces.Add(itm.GetType().Namespace);

                        yield return new CodeAttributeArgument(new CodePrimitiveExpression(itm));
                    }
                }
            }
        }

        private static IEnumerable<CodeAttributeArgument> GetPropertyCodeArguments(ICollection<string> namespaces, object obj)
        {
            var type = obj.GetType();

            namespaces.Add(type.Namespace);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetValue(obj) != default)
                .ToList();

            foreach (var pi in properties)
            {
                if (pi.DeclaringType is null)
                    throw new ArgumentException("Could nto get type of property in obj!", nameof(obj));

                namespaces.Add(pi.DeclaringType.Namespace);

                var value = pi.GetValue(obj);
                var valType = value.GetType();

                namespaces.Add(value.GetType().Namespace);

                if (valType.IsPrimitive || valType == typeof(string))
                {
                    namespaces.Add(valType.Namespace);

                    yield return new CodeAttributeArgument(pi.Name, new CodePrimitiveExpression(value));
                }
                else if (value is IEnumerable collection)
                {
                    namespaces.Add(collection.GetType().Namespace);

                    foreach (var itm in collection)
                    {
                        namespaces.Add(itm.GetType().Namespace);

                        yield return new CodeAttributeArgument(pi.Name, new CodePrimitiveExpression(itm));
                    }
                }
            }
        }

        private static string GetNameWithoutGenericArity(Type t)
        {
            var name = t.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }

        private static List<PropertyInfo> GetPublicReadWriteProperties(Type type)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .ToList();

            return props;
        }
    }
}
