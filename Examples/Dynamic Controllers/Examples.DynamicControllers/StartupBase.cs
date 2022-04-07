using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using APIBlox.AspNetCore.Contracts;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Examples
{
    /// <summary>
    ///     Class Startup.
    /// </summary>
    public abstract class StartupBase
    {
        private const string SiteTitle = "APIBlox Example: DynamiControllers";
        private const string Version = "v1";
        private readonly IWebHostEnvironment _environment;

        private string _dynamicControllersXmlFile;
        private Assembly _dynamicControllersAssembly;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartupBase" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        protected StartupBase(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <summary>
        ///     Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            var startupAssembly = Assembly.GetAssembly(GetType());

            services
                .AddServerFaults()

                //
                // Instead of having to manually add to service collection.
                .AddReferencedInjectableServices(Program.StartupLogger)

                //
                //  DynamicControllers and configuration
                .AddDynamicControllerConfigurations(Program.StartupLogger,
                    GetType(),
                    _environment.IsProduction(),
                    _environment.IsProduction(),
                    true,
                    factory =>
                    {
                        factory.AdditionalAssemblyReferences.Add(startupAssembly);

                        return BuildTemplates(new List<IComposedTemplate>());
                    },
                    (factory, xml, ass, csFiles) =>
                    {
                        _dynamicControllersXmlFile = xml;
                        _dynamicControllersAssembly = ass;

                        if (factory.Errors != null)
                            throw new Exception("arg");
                    },
                    Path.GetDirectoryName(startupAssembly.Location)
                )
            #if DEBUG
                .AddMvc()
            #else
                .AddMvcCore()
            #endif
                .AddApplicationPart(_dynamicControllersAssembly)

                //
                // Resource Validator.
                .AddValidateResourceActionFilter()

                //
                // Handles cancellation token cancelled.
                .AddOperationCancelledExceptionFilter()

                //
                // Pagination
                .AddEnsurePaginationResultActionFilter(Program.StartupLogger)

                // Location header
                .AddPostLocationHeaderResultFilter()

                //
                // Make sure all results are camel cased.
                .AddCamelCaseResultsOptions()

                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining(GetType()));

#if DEBUG
            services.AddSwaggerExampleFeatures(SiteTitle, Version, _dynamicControllersXmlFile);
#endif
        }

        /// <summary>
        ///     Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public virtual void Configure(IApplicationBuilder app)
        {
            //
            // Handle any and all server (500) errors with a defined structure.
            app.UseServerFaults();

            app.UseHsts();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

#if DEBUG
            app.UseSwaggerExampleFeatures(SiteTitle, Version);
#endif
        }

        /// <summary>
        ///     Builds the templates.
        /// </summary>
        /// <returns>IEnumerable&lt;IComposedTemplate&gt;.</returns>
        protected abstract IEnumerable<IComposedTemplate> BuildTemplates(List<IComposedTemplate> templates);
    }
}
