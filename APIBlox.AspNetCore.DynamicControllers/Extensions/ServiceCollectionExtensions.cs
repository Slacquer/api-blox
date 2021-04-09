using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds and configures dynamic controller configurations using the <see cref="DynamicControllerFactory" />.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="startup">The startup type.</param>
        /// <param name="useCached">if set to <c>true</c> [use caching].</param>
        /// <param name="releaseConfiguration">if set to <c>true</c> [release configuration].</param>
        /// <param name="addControllerComments">
        ///     if set to <c>true</c> Summary comments from the request object of the first
        ///     controller action added will be included as controller summary comments..
        /// </param>
        /// <param name="preCompile">
        ///     The pre compile function.  If caching and cache is found then this function will not be
        ///     called.
        /// </param>
        /// <param name="postCompile">The post compile action.</param>
        /// <param name="xmlFallbackPaths">The XML fallback paths.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddDynamicControllerConfigurations(this IServiceCollection services,
            ILoggerFactory loggerFactory, Type startup, bool useCached, bool releaseConfiguration, bool addControllerComments,
            Func<DynamicControllerFactory, IEnumerable<IComposedTemplate>> preCompile,
            Action<DynamicControllerFactory, string, Assembly, string[]> postCompile,
            params string[] xmlFallbackPaths
        )
        {
            var log = loggerFactory.CreateLogger(nameof(AddDynamicControllerConfigurations));
            var caller = Assembly.GetAssembly(startup);
            var assemblyName = $"{caller?.GetName().Name}.Controllers";
            var outputPath = Path.Combine(Path.GetDirectoryName(caller?.Location) ?? throw new InvalidOperationException(), "DynamicControllers");
            var dll = new FileInfo(Path.Combine(outputPath, $"{assemblyName}.dll"));
            var xmlFile = new FileInfo(Path.Combine(outputPath, $"{assemblyName}.xml"));
            var factory = new DynamicControllerFactory(loggerFactory, assemblyName, releaseConfiguration, addControllerComments);

            XmlDocumentationExtensions.FallbackPaths = xmlFallbackPaths.ToList();

            if (useCached && CacheFilesExist(log, dll, xmlFile))
            {
                postCompile?.Invoke(factory, xmlFile.FullName, Assembly.LoadFrom(dll.FullName), null);
                return services;
            }

            var templates = preCompile?.Invoke(factory);

            var templatesToCompile = TemplatesCheck(loggerFactory, templates);

            if (templatesToCompile is null)
                return services;

            var outputAss = factory.Compile(outputPath, templatesToCompile);

            var (_, _, xml, csFiles) = factory.OutputFiles;

            postCompile?.Invoke(factory, xml, outputAss, csFiles);

            return services;
        }

        private static bool CacheFilesExist(ILogger log, FileSystemInfo dll, FileSystemInfo xml)
        {
            if (dll.Exists)
            {
                var dn = dll.FullName;
                var xn = xml.FullName;

                log.LogInformation(() => $"Using cached dynamic controller assembly file: '{dn}'");

                if (!xml.Exists)
                    log.LogWarning(() => $"Caching is being used however, XML file '{xn}' not found.");

                return true;
            }

            log.LogWarning(() =>
                "Configured to use caching, however dynamic controller assembly " +
                $"file '{dll.FullName}' was not found, therefore cache will NOT be used."
            );

            return false;
        }

        private static IComposedTemplate[] TemplatesCheck(ILoggerFactory loggerFactory, IEnumerable<IComposedTemplate> templates)
        {
            var composedTemplates = templates?.ToArray();

            var invalid = composedTemplates is null || !composedTemplates.Any();

            if (!invalid)
                return composedTemplates;

            loggerFactory.CreateLogger(nameof(AddDynamicControllerConfigurations))
                .LogCritical(() => "Controller templates collection is empty.  Nothing to do!");

            return null;
        }
    }
}
