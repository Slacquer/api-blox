using System.Reflection;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Exceptions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

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
