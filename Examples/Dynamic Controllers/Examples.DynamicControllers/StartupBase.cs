using System.Collections.Generic;
using System.Reflection;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Contracts;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Examples
{
    /// <summary>
    ///     Class Startup.
    /// </summary>
    public abstract class StartupBase
    {
        private const string SiteTitle = "APIBlox Example: DynamiControllers";
        private const string Version = "v1";
        private readonly IHostingEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;

        private readonly string _assemblyFileAndName;
        private string _dynamicControllersXmlFile;
        private Assembly _dynamicControllersAssembly;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartupBase" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="assemblyFileAndName">Name of the assembly file and.</param>
        protected StartupBase(IHostingEnvironment environment, ILoggerFactory loggerFactory,
            string assemblyFileAndName = "DynamicControllersAssembly"
        )
        {
            _environment = environment;
            _loggerFactory = loggerFactory;
            _assemblyFileAndName = assemblyFileAndName;
        }

        /// <summary>
        ///     Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            DynamicControllerFactory.FactoryCreated += (s, e) =>
                ((DynamicControllerFactory) s).AdditionalAssemblyReferences.Add(Assembly.GetAssembly(GetType()));

            services
                .AddServerFaults()

                //
                // Instead of having to manually add to service collection.
                .AddReferencedInjectableServices(_loggerFactory)

                //
                //  DynamicControllers and configuration
                .AddDynamicControllerConfigurations(_loggerFactory,
                    _environment.IsProduction(),
                    _assemblyFileAndName,
                    () => BuildTemplates(new List<IComposedTemplate>()),
                    (factory, xml, ass) =>
                    {
                        _dynamicControllersXmlFile = xml;
                        _dynamicControllersAssembly = ass;
                    }
                )
            #if DEBUG
                .AddMvc()
            #else
                .AddMvcCore()
            #endif
                .AddPostLocationHeaderResultFilter()
                .AddApplicationPart(_dynamicControllersAssembly)

                //
                // Handles cancellation token cancelled.
                .AddOperationCancelledExceptionFilter()

                //
                // Pagination
                .AddEnsurePaginationResultActionFilter(_loggerFactory, defaultPageSize: 10)

                //
                // Resource Validator.
                .AddValidateResourceActionFilter()

                //
                // Make sure all results are camel cased.
                .AddCamelCaseResultsOptions()
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining(GetType()))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

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

            app.UseMvc();

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
