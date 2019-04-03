using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace APIBlox.AspNetCore.Extensions
{
    /// <summary>
    ///     Class MvcBuilderExtensions.
    /// </summary>
    public static class DynamicControllerFactoryExtensions
    {
        /// <summary>
        ///     Compiles <see cref="IComposedTemplate" /> and adds a new <seealso cref="AssemblyPart" /> to the
        ///     <seealso cref="IMvcBuilder.PartManager" />
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="useCache">if set to <c>true</c> [use cache].  <seealso cref="DynamicControllerFactory.Compile(string,bool,APIBlox.AspNetCore.Contracts.IComposedTemplate[])"/></param>
        /// <param name="templates">The templates.</param>
        /// <returns>DynamicControllerFactory.</returns>
        /// <exception cref="TemplateCompilationException"></exception>
        public static DynamicControllerFactory Compile(this DynamicControllerFactory factory,
            IMvcBuilder builder, string outputFile, bool useCache, params IComposedTemplate[] templates
        )
        {
            var ass = factory.Compile(outputFile, useCache, templates);

            if (ass is null || factory.Errors != null)
                throw new TemplateCompilationException(factory.Errors);

            builder.ConfigureApplicationPartManager(pm =>
                {
                    var part = new AssemblyPart(Assembly.LoadFrom(ass.FullName));

                    pm.ApplicationParts.Add(part);
                }
            );

            return factory;
        }


        public static IMvcBuilder AddComposeControllers(this IMvcBuilder builder,
            ILoggerFactory loggerFactory, IHostingEnvironment environment,
            Action<DynamicControllerFactory, List<IComposedTemplate>> configure, 
            out string dynamicControllersXmlFile,
            string assemblyFileAndName = "DynamicControllersAssembly")
        {
            var factory = new DynamicControllerFactory(loggerFactory,
                assemblyFileAndName,
                environment.IsProduction()
            );

            var composed = new List<IComposedTemplate>();
            configure(factory, composed);

            var outputFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), assemblyFileAndName);

            factory.Compile(builder, outputFile, environment.IsProduction(), composed.ToArray());

            var (_, _, xml) = factory.OutputFiles;

            dynamicControllersXmlFile = xml;

            return builder;
        }

        public static IMvcCoreBuilder AddComposeControllers(this IMvcCoreBuilder builder,
            ILoggerFactory loggerFactory, IHostingEnvironment environment,
            Action<DynamicControllerFactory, List<IComposedTemplate>> configure, 
            out string dynamicControllersXmlFile,
            string assemblyFileAndName = "DynamicControllersAssembly")
        {
            var factory = new DynamicControllerFactory(loggerFactory,
                assemblyFileAndName,
                environment.IsProduction()
            );

            var composed = new List<IComposedTemplate>();
            configure(factory, composed);

            var outputFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), assemblyFileAndName);

            factory.Compile(builder, outputFile, environment.IsProduction(), composed.ToArray());

            var (_, _, xml) = factory.OutputFiles;

            dynamicControllersXmlFile = xml;

            return builder;
        }

        /// <summary>
        ///     Compiles <see cref="IComposedTemplate" /> and adds a new <seealso cref="AssemblyPart" /> to the
        ///     <seealso cref="IMvcCoreBuilder.PartManager" />
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="useCache">if set to <c>true</c> [use cache].  <seealso cref="DynamicControllerFactory.Compile(string,bool,APIBlox.AspNetCore.Contracts.IComposedTemplate[])"/></param>
        /// <param name="templates">The templates.</param>
        /// <returns>DynamicControllerFactory.</returns>
        /// <exception cref="TemplateCompilationException"></exception>
        public static DynamicControllerFactory Compile(this DynamicControllerFactory factory,
            IMvcCoreBuilder builder, string outputFile, bool useCache, params IComposedTemplate[] templates
        )
        {
            var ass = factory.Compile(outputFile, useCache, templates);

            if (ass is null || factory.Errors != null)
                throw new TemplateCompilationException(factory.Errors);

            builder.ConfigureApplicationPartManager(pm =>
                {
                    var part = new AssemblyPart(Assembly.LoadFrom(ass.FullName));

                    pm.ApplicationParts.Add(part);
                }
            );

            return factory;
        }
    }
}
