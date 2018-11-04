using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace APIBlox.AspNetCore.Extensions
{
    /// <summary>
    ///     Class DynamicControllerTemplateExtensions.
    /// </summary>
    public static class DynamicControllerTemplateExtensions
    {
        ////var controllerName = "MyController";
        ////var requestObjectType = "int";
        ////var responseObjectType = "long";
        ////var parameters = "int p1, long p2";
        ////var contents = File.ReadAllText(@".\getController.txt");

        ////contents = contents.Replace("@GetController", controllerName)
        ////.Replace("@RequestObj", requestObjectType)
        ////.Replace("@ResponseObj", responseObjectType)
        ////.Replace("@params", parameters);

        ////var types = CompileContents(contents);

        /// <summary>
        ///     The beginning of a new way of thinking...
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="failures">The failures.</param>
        /// <param name="assemblyName">The name space.</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <returns>Type[].</returns>
        public static Type[] ToTypes(this string contents,
            out IEnumerable<string> failures,
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

            var csSyntaxTree = new[] {CSharpSyntaxTree.ParseText(contents)};

            var compilation = CSharpCompilation.Create(assemblyName, csSyntaxTree, lst, csOptions);

            using (var ms = new MemoryStream())
            {
                var emitResult = compilation.Emit(ms);

                failures = null;

                if (!emitResult.Success)
                {
                    var diagnostics = emitResult.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error
                    );

                    var errors = diagnostics.Select(diagnostic =>
                            $"{diagnostic.Id}: {diagnostic.GetMessage()}"
                        )
                        .ToList();

                    failures = errors;

                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());

                return assembly.GetExportedTypes();
            }
        }
    }
}
