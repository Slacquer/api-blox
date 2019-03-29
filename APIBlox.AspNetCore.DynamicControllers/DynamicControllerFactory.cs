using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        /// <summary>
        ///     Makes a controller given a template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="debug">if set to <c>true</c> compiles in [debug].</param>
        /// <returns>System.ValueTuple&lt;Type, IEnumerable&lt;System.String&gt;&gt;.</returns>
        public static (IEnumerable<Type>, IEnumerable<string>) Make(string template,
            string assemblyName = "DynamicControllers",
            bool debug = true
        )
        {
            // Found the following piece of gold at...
            // https://github.com/dotnet/roslyn/wiki/Runtime-code-generation-using-Roslyn-compilations-in-.NET-Core-App
            var lst = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")

                //
                //
                .ToString()
                .Split(";", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => MetadataReference.CreateFromFile(s))
                .ToArray();

            var csOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: debug
                    ? OptimizationLevel.Debug
                    : OptimizationLevel.Release
            );

            var csSyntaxTree = new[] { CSharpSyntaxTree.ParseText(template) };

            var compilation = CSharpCompilation.Create(assemblyName, csSyntaxTree, lst, csOptions);

            using (var ms = new MemoryStream())
            {
                var emitResult = compilation.Emit(ms);

                if (emitResult.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    //var assembly = Assembly.Load(ms.ToArray());

                    var assembly = AppDomain.CurrentDomain.Load(ms.ToArray());

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
            public static (string, string[]) ToInputParams(Type obj)
            {
                const string template = "@att @p";
                var props = obj.GetProperties(BindingFlags.Public  | BindingFlags.Instance).ToList();

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
                    parameters.Append(temp.Replace("@p", $"{pi.PropertyType.Name} {pi.Name}{v}"));
                }

                return (parameters.ToString(), namespaces.ToArray());
            }

            /// <summary>
            ///     Given a type this will get a types name and namespaces.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>System.ValueTuple&lt;System.String, System.String[]&gt;.</returns>
            public static (string, string[]) ToStringWithNamespaces(Type obj)
            {
                var name = GetNameWithoutGenericArity(obj);
                var ns = obj.Namespace;
                var namespaces = new List<string> { ns };

                var result = new StringBuilder();
                result.Append($"{name}");

                if (obj.IsGenericType)
                {
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
                }

                return (result.ToString(), namespaces.ToArray());
            }
        }
    }
}
