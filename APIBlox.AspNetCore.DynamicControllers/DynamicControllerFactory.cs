using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace APIBlox.AspNetCore
{
    /// <summary>
    ///     Class DynamicControllerTemplateExtensions.
    /// </summary>
    public class DynamicControllerFactory
    {
        private readonly string _assemblyOutputName;
        private readonly bool _production;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicControllerFactory" /> class.
        /// </summary>
        /// <param name="assemblyOutputName">Name of the assembly output.</param>
        /// <param name="production">if set to <c>true</c> [production].</param>
        public DynamicControllerFactory(string assemblyOutputName, bool production)
        {
            _assemblyOutputName = assemblyOutputName;
            _production = production;
        }

        /// <summary>
        ///     Compiles a template into a controller.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>System.ValueTuple&lt;Type, IEnumerable&lt;System.String&gt;&gt;.</returns>
        public (IEnumerable<Type>, IEnumerable<string>) Compile(string template)
        {
            // Found the following piece of gold at...
            // https://github.com/dotnet/roslyn/wiki/Runtime-code-generation-using-Roslyn-compilations-in-.NET-Core-App
            var lst = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
                .ToString()
                .Split(";", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => MetadataReference.CreateFromFile(s))
                .ToArray();

            var csOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: _production
                    ? OptimizationLevel.Release
                    : OptimizationLevel.Debug
            );

            var csSyntaxTree = new[] { CSharpSyntaxTree.ParseText(template) };

            var compilation = CSharpCompilation.Create(_assemblyOutputName, csSyntaxTree, lst, csOptions);

            using (var ms = new MemoryStream())
            {
                var emitResult = compilation.Emit(ms);

                if (emitResult.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(ms.ToArray());

                    return (assembly.GetExportedTypes(), null);
                }

                var diagnostics = emitResult.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error
                );

                var failures = diagnostics.Select(diagnostic =>
                    $"{diagnostic.Id}: {diagnostic.GetMessage()}"
                ).ToArray();

                return (null, failures);
            }
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
            ///     Given a type this will get a types name and namespaces.  If a generic type, then the first argument is used as the type name.
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

                return $"new {obj.Name}{{{ setters }}}";
            }

            private static List<PropertyInfo> GetPublicReadWriteProperties(Type type)
            {
                return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite)
                    .ToList();
            }
        }
    }
}
