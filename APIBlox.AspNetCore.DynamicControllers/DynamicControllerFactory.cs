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

        private static string GetNameWithoutGenericArity(Type t)
        {
            var name = t.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }

        private static string BuildAtt(IModelNameProvider att, bool isRoute = true)
        {
            var preFix = isRoute ? "Route" : "Query";
            return att.Name is null ? $"[From{preFix}]" : $"[From{preFix}(Name = \"{att.Name}\")]";
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
            public static (string, string[]) GetInputParamsWithNamespaces(Type obj)
            {
                const string template = "@att @p";
                var props = GetPublicReadWriteProperties(obj);

                var namespaces = new List<string>();
                var parameters = new StringBuilder();

                for (var index = 0; index < props.Count; index++)
                {
                    var pi = props[index];

                    var qryAtt = pi.GetCustomAttributes(false).FirstOrDefault(t => t is FromQueryAttribute) as FromQueryAttribute;
                    var routeAtt = pi.GetCustomAttributes(false).FirstOrDefault(t => t is FromRouteAttribute) as FromRouteAttribute;

                    var temp = qryAtt is null && routeAtt is null
                        ? template.Replace("@att", "")
                        : template.Replace("@att", qryAtt is null ? BuildAtt(routeAtt) : BuildAtt(qryAtt, false));

                    if (!namespaces.Contains(pi.PropertyType.Namespace))
                        namespaces.Add(pi.PropertyType.Namespace);

                    var v = index < props.Count - 1 ? ", " : "";
                    parameters.Append(temp.Replace("@p", $"{pi.PropertyType.Name} {pi.Name.ToCamelCase()}{v}"));
                }

                return (parameters.ToString(), namespaces.ToArray());
            }

            /// <summary>
            ///     Given a type this will get a types name and namespaces.  If a generic type, then the first argument is used as the
            ///     type name.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>System.ValueTuple&lt;System.String, System.String, System.String[]&gt;.</returns>
            public static (string, string, string[]) GetNameWithNamespaces(Type obj)
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
            public static string GetNewObject(Type obj)
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

            private static List<PropertyInfo> GetPublicReadWriteProperties(Type type)
            {
                return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite)
                    .ToList();
            }
        }

        #endregion
    }
}
