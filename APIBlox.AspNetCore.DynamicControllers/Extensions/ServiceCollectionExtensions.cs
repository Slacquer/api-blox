using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Exceptions;
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
        ///     Adds and configures dynamic controller configurations using the <see cref="DynamicControllerFactory"/>.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="useCached">if set to <c>true</c> [use cached].</param>
        /// <param name="assemblyFileAndName">Name of the assembly file and.</param>
        /// <param name="configureTemplates">The configure.</param>
        /// <param name="xmlResult">The XML result.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="TemplateCompilationException"></exception>
        public static IServiceCollection AddDynamicControllerConfigurations(this IServiceCollection services,
            ILoggerFactory loggerFactory, bool useCached, string assemblyFileAndName,
            Func<IEnumerable<IComposedTemplate>> configureTemplates, Action<DynamicControllerFactory, string, Assembly> xmlResult
        )
        {
            var factory = new DynamicControllerFactory(loggerFactory,
                assemblyFileAndName,
                useCached
            );

            var caller = Assembly.GetCallingAssembly();
            var outputFile = Path.Combine(Path.GetDirectoryName(caller.Location), assemblyFileAndName);
            
            var templates = configureTemplates().ToArray();

            var outputAss = factory.Compile(
                outputFile,
                useCached,
                templates.ToArray()
            );
            
            var (_, _, xml) = factory.OutputFiles;

            xmlResult(factory, xml, outputAss);

            return services;
        }
    }
}
