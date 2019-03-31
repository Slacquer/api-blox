using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace APIBlox.AspNetCore
{
    /// <summary>
    ///     Class DynamicControllerTemplateExtensions.
    /// </summary>
    public class DynamicControllerFactory
    {
        private readonly string _assemblyName;
        private readonly bool _production;
        private readonly PortableExecutableReference[] _references;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicControllerFactory" /> class.
        /// </summary>
        /// <param name="assemblyName">Name of the final assembly.</param>
        /// <param name="production">if set to <c>true</c> [production] otherwise [debug].</param>
        /// <exception cref="ArgumentNullException">assemblyName</exception>
        public DynamicControllerFactory(string assemblyName, bool production = false)
        {
            if (assemblyName.IsEmptyNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(assemblyName));

            _assemblyName = assemblyName;
            _production = production;

            // Found the following piece of gold at...
            // https://github.com/dotnet/roslyn/wiki/Runtime-code-generation-using-Roslyn-compilations-in-.NET-Core-App
            _references = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
                .ToString()
                .Split(";", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => MetadataReference.CreateFromFile(s))
                .ToArray();
        }

        /// <summary>
        ///     Gets the compilation errors.
        /// </summary>
        /// <value>The compilation errors.</value>
        public IEnumerable<string> CompilationErrors { get; private set; } = new List<string>();

        /// <summary>
        ///     Gets the compilation warnings.
        /// </summary>
        /// <value>The compilation warnings.</value>
        public IEnumerable<string> CompilationWarnings { get; private set; } = new List<string>();

        /// <summary>
        ///     Compiles <see cref="IComposedTemplate"/> to the specified assemblyOutputPath.
        /// <para>
        ///     When null is returned, then errors have been generated, check the <see cref="CompilationErrors"/> property.
        /// </para>
        /// </summary>
        /// <param name="assemblyOutputPath">The assembly output path.</param>
        /// <param name="templates">The templates.</param>
        /// <returns>FileInfo.</returns>
        /// <exception cref="ArgumentNullException">assemblyOutputPath</exception>
        public FileInfo Compile(string assemblyOutputPath, params IComposedTemplate[] templates)
        {
            if (assemblyOutputPath.IsEmptyNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(assemblyOutputPath));

            if (!Directory.Exists(assemblyOutputPath))
                Directory.CreateDirectory(assemblyOutputPath);

            return EmitToFile(assemblyOutputPath, templates);
        }

        /// <summary>
        ///     Compiles <see cref="IComposedTemplate"/> to a collection of types.
        /// <para>
        ///     When null is returned, then errors have been generated, check the <see cref="CompilationErrors"/> property.
        /// </para>
        /// </summary>
        /// <param name="templates">The templates.</param>
        /// <returns>IEnumerable&lt;Type&gt;.</returns>
        public IEnumerable<Type> Compile(params IComposedTemplate[] templates)
        {
            return EmitToTypes(templates);
        }

        private IEnumerable<Type> EmitToTypes(params IComposedTemplate[] templates)
        {
            var csOptions = ResetErrorsAndGetSyntaxTree(templates, out var csSyntaxTree);

            var compilation = CSharpCompilation.Create(_assemblyName, csSyntaxTree, _references, csOptions);

            using (var ms = new MemoryStream())
            {
                var emitResult = compilation.Emit(ms);

                if (emitResult.Success)
                {
                    CheckAndSetWarnings(emitResult);

                    ms.Seek(0, SeekOrigin.Begin);

                    var assembly = Assembly.Load(ms.ToArray());

                    return assembly.GetExportedTypes();
                }

                CheckAndSetFailures(emitResult);

                return null;
            }
        }


        private FileInfo EmitToFile(string outputFolder, params IComposedTemplate[] templates)
        {
            var csOptions = ResetErrorsAndGetSyntaxTree(templates, out var csSyntaxTree);

            var compilation = CSharpCompilation.Create(_assemblyName, csSyntaxTree, _references, csOptions);

            var dll = Path.Combine(outputFolder, $"{_assemblyName}.dll");
            var pdb = Path.Combine(outputFolder, $"{_assemblyName}.pdb");
            var xml = Path.Combine(outputFolder, $"{_assemblyName}.xml");

            var emitResult = compilation.Emit(dll, _production ? null : pdb, xml);

            if (emitResult.Success)
            {
                CheckAndSetWarnings(emitResult);
                return new FileInfo(dll);
            }

            CheckAndSetFailures(emitResult);

            return null;
        }

        private CSharpCompilationOptions ResetErrorsAndGetSyntaxTree(IComposedTemplate[] templates, out IEnumerable<SyntaxTree> csSyntaxTree)
        {
            if (templates is null || !templates.Any())
                throw new ArgumentNullException(nameof(templates));

            CompilationErrors = null;

            var csOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: _production
                    ? OptimizationLevel.Release
                    : OptimizationLevel.Debug
            );

            csSyntaxTree = templates.Select(t => CSharpSyntaxTree.ParseText(t.Content));
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

            CompilationErrors = failures;
        }

        private void CheckAndSetWarnings(EmitResult emitResult)
        {
            var diagnostics = emitResult.Diagnostics.Where(diagnostic =>
                !diagnostic.IsSuppressed
            );

            var warnings = diagnostics.Select(diagnostic =>
                $"{diagnostic.Id}: {diagnostic.GetMessage()}"
            ).ToArray();

            CompilationWarnings = warnings;
        }

        #region -    Nested type: Helpers    -

        /// <summary>
        ///     Class Helpers.
        /// </summary>
        public static class Helpers
        {
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
                        .Replace("@p", $"{GetPropertyTypeAndValue(pi.PropertyType, pi.Name)},")
                    );
                }

                var paramsString = string.Join(Environment.NewLine, parameters);

                return (paramsString, namespaces.ToArray());
            }

            private static string GetPropertyTypeAndValue(Type prop, string propName)
            {
                var (nullable, name) = IsOfNullableType(prop);

                return !nullable
                    ? $"{prop.Name} {propName.ToCamelCase()}"
                    : $"{name}? {propName.ToCamelCase()}";
            }

            private static (bool, string) IsOfNullableType(Type o)
            {
                var nullable = o.IsGenericType && o.GetGenericTypeDefinition() == typeof(Nullable<>);

                return !nullable
                    ? (false, o.Name)
                    : (true, o.GetGenericArguments().First().Name);
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
            ///  Gets the name without generic arity.
            /// </summary>
            /// <param name="t">The t.</param>
            /// <returns>System.String.</returns>
            public static string GetNameWithoutGenericArity(Type t)
            {
                var name = t.Name;
                var index = name.IndexOf('`');
                return index == -1 ? name : name.Substring(0, index);
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

                for (var index = 0; index < writeProps.Count; index++)
                {
                    var prop = writeProps[index];
                    var value = prop.GetValue(attribute);

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

                for (var index = 0; index < ctorArgs.Count; index++)
                {
                    var cp = ctorArgs[index];
                    var cpi = readProps.First(p => p.Name.EqualsEx(cp.Name));

                    var value = cpi.GetValue(attribute);

                    if (!namespaces.Contains(cp.ParameterType.Namespace))
                        namespaces.Add(cp.ParameterType.Namespace);

                    if (value is bool b)
                        value = b.ToString().ToLower();

                    var comma = index == ctorArgs.Count - 1 ? "" : ", ";

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

            public static void ValidateResponseType(Type response)
            {
                if (response.IsPublic)
                    return;

                throw new ArgumentException($"{response.Name} protection level must be public.");
            }

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
        }

        #endregion
    }
}
