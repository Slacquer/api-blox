using System;
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
        private readonly bool _production;
        private readonly ILogger<DynamicControllerFactory> _log;

        /// <summary>
        ///     A collection of additional assemblies they may be required when compiling.
        /// </summary>
        /// <value>The additional assembly references.</value>
        public List<Assembly> AdditionalAssemblyReferences { get; } = new List<Assembly>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicControllerFactory"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="production">if set to <c>true</c> [production].</param>
        /// <exception cref="ArgumentNullException">assemblyName</exception>
        public DynamicControllerFactory(ILoggerFactory loggerFactory, string assemblyName, bool production = false)
        {
            if (assemblyName.IsEmptyNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(assemblyName));

            _assemblyName = assemblyName;
            _production = production;
            _log = loggerFactory.CreateLogger<DynamicControllerFactory>();
        }

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
        public (string, string, string) OutputFiles { get; private set; }

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
        /// <param name="useCache">if set to <c>true</c> [use cache].</param>
        /// <param name="templates">The templates.</param>
        /// <returns>Assembly.</returns>
        /// <exception cref="ArgumentNullException">assemblyOutputPath</exception>
        public Assembly Compile(string assemblyOutputPath, bool useCache, params IComposedTemplate[] templates)
        {
            if (assemblyOutputPath.IsEmptyNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(assemblyOutputPath));

            if (!Directory.Exists(assemblyOutputPath))
                Directory.CreateDirectory(assemblyOutputPath);

            var fi = EmitToFile(assemblyOutputPath, useCache, templates);

            return !(fi is null) && fi.Exists ? Assembly.LoadFile(fi.FullName) : null;
        }

        /// <summary>
        ///     Compiles 1 or more <see cref="IComposedTemplate" /> to an assembly in memory.
        ///     <para>
        ///         When null is returned, then errors have been generated, check the <see cref="Errors" /> property.
        ///     </para>
        /// </summary>
        /// <param name="templates">The templates.</param>
        /// <returns>IEnumerable&lt;Type&gt;.</returns>
        public Assembly Compile(params IComposedTemplate[] templates)
        {
            return EmitToAssembly(templates);
        }

        /// <summary>
        ///     Given a type this will generate method input parameters in string form along with namespaces.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.ValueTuple&lt;System.String, System.String[]&gt;.</returns>
        public static (string, string[]) WriteInputParamsWithNamespaces(Type obj)
        {
            const string template = "@space@att @p";
            var props = GetPublicReadWriteProperties(obj);

            var namespaces = new List<string>();
            var parameters = new List<string>();

            for (var index = 0; index < props.Count; index++)
            {
                var pi = props[index];

                var temp = template.Replace("@att", GetAttributesAndValues(namespaces, pi));

                if (!namespaces.Contains(pi.PropertyType.Namespace))
                    namespaces.Add(pi.PropertyType.Namespace);

                var space = index == 0 ? "" : "            ";

                parameters.Add(temp
                    .Replace("@space", space)
                    .Replace("@p", $"{GetPropertyTypeAndValue(namespaces, pi.PropertyType, pi.Name)},")
                );
            }

            var paramsString = string.Join(Environment.NewLine, parameters);

            return (paramsString, namespaces.ToArray());
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
                var xml = pi.GetSummary();

                var space = index == 0 ? "" : "        ";

                parameters.Add(template
                    .Replace("@space", space)
                    .Replace("@pName", pi.Name.ToCamelCase())
                    .Replace("@pComment", xml)
                );
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
            if (response.IsPublic)
                return;

            throw new ArgumentException($"{response.Name} protection level must be public.");
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

        /// <summary>
        ///     Gets the name without generic arity.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>System.String.</returns>
        private static string GetNameWithoutGenericArity(Type t)
        {
            var name = t.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }

        private static string GetPropertyTypeAndValue(ICollection<string> namespaces, Type prop, string propName)
        {
            var (nullable, name) = IsOfNullableType(prop);

            if (!nullable)
                if (prop.IsGenericType)
                {
                    var builder = new StringBuilder();

                    builder.Append("<");

                    var args = prop.GetGenericArguments();

                    for (var index = 0; index < args.Length; index++)
                    {
                        var type = args[index];

                        if (!namespaces.Contains(type.Namespace))
                            namespaces.Add(type.Namespace);

                        var tName = type.Name;

                        if (type.IsGenericType)
                            tName = GetPropertyTypeAndValue(namespaces, type, null);

                        builder.Append(index == args.Length - 1 ? $"{tName}" : $"{tName},");
                    }

                    builder.Append(">");

                    name = $"{GetNameWithoutGenericArity(prop)}{builder}";
                }

            if (nullable)
                return propName is null ? name : $"{name}? {propName.ToCamelCase()}";

            if (prop.IsGenericType)
                return propName is null ? name : $"{name} {propName.ToCamelCase()}";

            return $"{prop.Name} {propName.ToCamelCase()}";
        }

        private static (bool, string) IsOfNullableType(Type o)
        {
            var nullable = o.IsGenericType && o.GetGenericTypeDefinition() == typeof(Nullable<>);

            return !nullable
                ? (false, o.Name)
                : (true, o.GetGenericArguments().First().Name);
        }

        private static string GetAttributesAndValues(ICollection<string> namespaces, MemberInfo pi)
        {
            var builder = new StringBuilder();

            var attributes = pi.GetCustomAttributes().ToList();

            foreach (var attribute in attributes)
            {
                var attType = attribute.GetType();

                if (!namespaces.Contains(attType.Namespace))
                    namespaces.Add(attType.Namespace);

                builder.Append("[");
                builder.Append($"{attType.Name.Replace("Attribute", "")}(");
                BuildAttributeConstructor(namespaces, attribute, builder);
                BuildAttributeWriteProperties(attribute, builder);

                builder.Append(")]");
            }

            var ret = builder.ToString();

            return ret;
        }

        private static void BuildAttributeWriteProperties(Attribute attribute, StringBuilder builder)
        {
            var type = attribute.GetType();
            var writeProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.GetValue(attribute) != default)
                .ToList();

            if (!writeProps.Any())
                return;

            if (!builder.ToString().EndsWith("("))
                builder.Append(", ");

            var argList = new List<string>();

            for (var index = 0; index < writeProps.Count; index++)
            {
                var prop = writeProps[index];
                var value = prop.GetValue(attribute);

                argList.Add(value.ToString());

                if (value is bool b)
                    value = b.ToString().ToLower();

                var comma = index == writeProps.Count - 1 ? "" : ", ";

                builder.Append(prop.PropertyType == typeof(string)
                    ? $"{prop.Name} = \"{value}\"{comma}"
                    : $"{prop.Name} = {value}{comma}"
                );
            }
        }

        private static void BuildAttributeConstructor(ICollection<string> namespaces, Attribute attribute, StringBuilder builder)
        {
            var type = attribute.GetType();
            var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();

            if (ctor is null)
                return;

            var ctorArgs = ctor.GetParameters().ToList();

            if (!ctorArgs.Any())
                return;

            var readProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetValue(attribute) != default)
                .ToList();

            var argList = new List<string>();

            for (var index = 0; index < ctorArgs.Count; index++)
            {
                var cp = ctorArgs[index];
                var cpi = readProps.FirstOrDefault(p => p.Name.EqualsEx(cp.Name));

                if (cpi is null)
                    throw new TemplateCompilationException(
                        new[] { $"Attribute {type.Name} does not have a GETTER, parser can NOT get current values for constructor!" }
                    );

                var value = cpi.GetValue(attribute);

                if (!namespaces.Contains(cp.ParameterType.Namespace))
                    namespaces.Add(cp.ParameterType.Namespace);

                argList.Add(value.ToString());

                if (value is bool b)
                    value = b.ToString().ToLower();

                var comma = index == ctorArgs.Count - 1 ? "" : ", ";

                // TODO does not work with arrays dumb ass., IE: new[]{"ur", "dumb"}

                // this is lame.
                if (cp.ParameterType == typeof(IEnumerable))
                {
                    if (cp.ParameterType == typeof(string))
                    {
                        var v = (IEnumerable<string>)value;
                        value = v.Select(s => $"\"{s}\"");
                    }
                    value = $"new[]{{{string.Join(",", value)}}}";
                }

                builder.Append(cp.ParameterType == typeof(string)
                    ? $"\"{value}\"{comma}"
                    : $"{value}{comma}"
                );
            }
        }

        private static List<PropertyInfo> GetPublicReadWriteProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .ToList();
        }

        private static void AddExistingArray(List<string> dest, IEnumerable<string> src)
        {
            if (src is null)
                return;

            dest.AddRange(src);
        }


        private Assembly EmitToAssembly(params IComposedTemplate[] templates)
        {
            Reset();

            var csOptions = GetSyntaxTree(templates, out var csSyntaxTree);

            var compilation = CSharpCompilation.Create(_assemblyName, csSyntaxTree, GetReferences(), csOptions);

            using (var ms = new MemoryStream())
            {
                var emitResult = compilation.Emit(ms);

                if (emitResult.Success)
                {
                    CheckAndSetWarnings(emitResult);

                    ms.Seek(0, SeekOrigin.Begin);

                    var assembly = Assembly.Load(ms.ToArray());

                    _log.LogInformation(() => $"Created dynamic controllers assembly: {assembly.FullName}");

                    return assembly;
                }

                _log.LogCritical(() => $"Could not create dynamic controllers assembly: {_assemblyName}");

                CheckAndSetFailures(emitResult);

                return null;
            }
        }

        private FileInfo EmitToFile(string outputFolder, bool useCache, params IComposedTemplate[] templates)
        {
            Reset();

            var dll = new FileInfo(Path.Combine(outputFolder, $"{_assemblyName}.dll"));
            var pdb = new FileInfo(Path.Combine(outputFolder, $"{_assemblyName}.pdb"));
            var xml = new FileInfo(Path.Combine(outputFolder, $"{_assemblyName}.xml"));

            OutputFiles = (dll.FullName, pdb.FullName, xml.FullName);

            if (useCache && OutputsCheck(dll, pdb, xml))
                return dll;

            var csOptions = GetSyntaxTree(templates, out var csSyntaxTree);
            var compilation = CSharpCompilation.Create(_assemblyName, csSyntaxTree, GetReferences(), csOptions);

            try
            {
                var emitResult = compilation.Emit(dll.FullName, _production ? null : pdb.FullName, xml.FullName);

                if (emitResult.Success)
                {
                    _log.LogInformation(() => $"Created dynamic controllers assembly file: {dll.FullName}");

                    CheckAndSetWarnings(emitResult);
                    return dll;
                }

                _log.LogCritical(() => $"Could not create dynamic controllers assembly file: {dll.FullName}");

                CheckAndSetFailures(emitResult);
            }
            catch (IOException ioEx)
            {
                if (dll.Exists)
                {
                    Warnings = new List<string> { ioEx.Message };
                    _log.LogWarning(() => $"Could not create dynamic controllers assembly file: {dll.FullName}.  Its in use!");
                }
                else
                {
                    Errors = new List<string> { ioEx.Message };
                    _log.LogCritical(() => $"Could not create dynamic controllers assembly file: {dll.FullName}.  Ex: {ioEx.Message}");
                }

                return dll;
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

                using (var ass = new AssemblyResolver())
                {
                    ass.LoadFromAssemblyPath(assembly.Location, out _);

                    lst.AddRange(ass.LoadedReferencedAssemblies
                        .Where(an => File.Exists(an.Location) && References.All(p => p.FilePath != an.Location))
                        .Select(a => MetadataReference.CreateFromFile(a.Location)));
                }
            }
            return lst;
        }

        private bool OutputsCheck(FileSystemInfo dll, FileSystemInfo pdb, FileSystemInfo xml)
        {
            if (dll.Exists && xml.Exists)
            {
                if (_production)
                {
                    var dllName = dll.FullName;
                    _log.LogInformation(() => $"Using cached dynamic controller assembly file: {dllName}");

                    return true;
                }

                if (!pdb.Exists)
                    _log.LogWarning(() => $"PDB Cache file {pdb.FullName} not found, cache not being used.");

                return false;
            }
            _log.LogWarning(() => $"DLL {dll.FullName} or XML file {xml.FullName} not found, cache not being used.");

            return false;
        }

        private void Reset()
        {
            OutputFiles = (null, null, null);
            Errors = null;
            Warnings = null;
            Controllers = null;
        }

        private CSharpCompilationOptions GetSyntaxTree(IComposedTemplate[] templates, out IEnumerable<SyntaxTree> csSyntaxTree)
        {
            if (templates is null || !templates.Any())
                throw new ArgumentNullException(nameof(templates));

            var csOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: _production
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
                var dc = new DynamicController(first.Name, first.Namespace, first.Route);

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

                results.Add(cg.Key, CSharpSyntaxTree.ParseText(dc.ToString()).GetRoot().NormalizeWhitespace().ToFullString());
            }

            Controllers = results;
            csSyntaxTree = Controllers.Select(r => CSharpSyntaxTree.ParseText(r.Value));

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
    }
}
