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
        /// <param name="addControllerComments">
        ///     if set to <c>true</c> Summary comments from the request object of the first
        ///     controller action added will be included as controller summary comments..
        /// </param>
        /// <param name="preCompile">The pre compile function.</param>
        /// <param name="postCompile">The post compile action.</param>
        /// <param name="xmlFallbackPaths">The XML fallback paths.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddDynamicControllerConfigurations(this IServiceCollection services,
            ILoggerFactory loggerFactory, Type startup, bool useCached, bool addControllerComments,
            Func<DynamicControllerFactory, IEnumerable<IComposedTemplate>> preCompile,
            Action<DynamicControllerFactory, string, Assembly> postCompile,
            params string[] xmlFallbackPaths
        )
        {
            var caller = Assembly.GetAssembly(startup);
            var assemblyName = $"{caller.GetName().Name}.Controllers";
            var outputFile = Path.Combine(Path.GetDirectoryName(caller.Location), "DynamicControllers");

            XmlDocumentationExtensions.FallbackPaths = xmlFallbackPaths.ToList();

            var factory = new DynamicControllerFactory(loggerFactory,
                assemblyName,
                useCached,
                addControllerComments
            );

            var templates = preCompile?.Invoke(factory);

            var templatesToCompile = TemplatesCheck(loggerFactory, templates);

            if (templatesToCompile is null)
                return services;

            var outputAss = factory.Compile(outputFile, useCached, templatesToCompile);

            var (_, _, xml) = factory.OutputFiles;

            postCompile?.Invoke(factory, xml, outputAss);

            return services;
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
