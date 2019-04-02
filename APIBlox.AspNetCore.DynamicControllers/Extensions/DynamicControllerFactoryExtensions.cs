using System.Reflection;
using APIBlox.AspNetCore.Contracts;
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
        ///     Compiles <see cref="IComposedTemplate"/> and adds a new <seealso cref="AssemblyPart"/> to the <seealso cref="IMvcBuilder.PartManager"/>
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="templates">The templates.</param>
        /// <returns>DynamicControllerFactory.</returns>
        /// <exception cref="APIBlox.AspNetCore.Extensions.TemplateCompilationException"></exception>
        public static DynamicControllerFactory Compile(this DynamicControllerFactory factory, 
            IMvcBuilder builder, string outputFile, params IComposedTemplate[] templates)
        {
            var ass = factory.Compile(outputFile, templates);

            if (ass is null || factory.CompilationErrors != null)
                throw new TemplateCompilationException(factory.CompilationErrors);

            builder.ConfigureApplicationPartManager(pm =>
                {
                    var part = new AssemblyPart(Assembly.LoadFrom(ass.FullName));
                    pm.ApplicationParts.Add(part);
                }
            );


            return factory;
        }

        /// <summary>
        ///     Compiles <see cref="IComposedTemplate"/> and adds a new <seealso cref="AssemblyPart"/> to the <seealso cref="IMvcCoreBuilder.PartManager"/>
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="templates">The templates.</param>
        /// <returns>DynamicControllerFactory.</returns>
        /// <exception cref="APIBlox.AspNetCore.Extensions.TemplateCompilationException"></exception>
        public static DynamicControllerFactory Compile(this DynamicControllerFactory factory, IMvcCoreBuilder builder, string outputFile, params IComposedTemplate[] templates)
        {
            var ass = factory.Compile(outputFile, templates);

            if (ass is null || factory.CompilationErrors != null)
                throw new TemplateCompilationException(factory.CompilationErrors);

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
