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
    public class Startup
    {
        private const string SiteTitle = "APIBlox Example: DynamiControllers";
        private const string Version = "v1";
        private readonly IHostingEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            _environment = environment;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        ///     Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddServerFaults()

                //
                // Instead of having to manually add to service collection.
                .AddReferencedInjectableServices(_loggerFactory)
                .AddMvc()

                //
                //  DynamicControllers and configuration
                .AddFullyDynamicConfiguration(_environment, out var dynamicControllersXmlFile)

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
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerExampleFeatures(SiteTitle, Version, dynamicControllersXmlFile);
        }

        /// <summary>
        ///     Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configure(IApplicationBuilder app)
        {
            //
            // Handle any and all server (500) errors with a defined structure.
            app.UseServerFaults();

            //
            // Good for testing how things respond (when things go too
            // quickly because your dev machine is such a monster!)
            app.UseSimulateWaitTime(_environment);

            app.UseHsts();

            app.UseMvc();

            app.UseSwaggerExampleFeatures(SiteTitle, Version);
        }
    }
}
