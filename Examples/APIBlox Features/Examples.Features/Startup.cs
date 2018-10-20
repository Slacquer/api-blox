
using System.IO;
using Examples.Contracts;
using Examples.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;

#if UseAPIBlox
using System.Reflection;
#endif

namespace Examples
{
    internal class Startup
    {
#if UseAPIBlox
        private readonly string[] _assemblyNames;
        private readonly string[] _assemblyPaths;
        private const string AboutErrorsUrl = "http://hey.look.at.me/errorcodes";
#endif

        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;


        private const string Version = "v1";
        private const string SiteTitle = "APIBlox Example: Features";

        public Startup(IConfiguration configuration, IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _environment = environment;
            _loggerFactory = loggerFactory;

#if UseAPIBlox
            _assemblyNames = new[]
            {
                "Examples."
            };
            _assemblyPaths = new[]
            {
                _environment.ContentRootPath,
                new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName,
                $"!{environment.ContentRootPath}\\**\\obj"
            };
#endif
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
#if UseAPIBlox
                //
                // Instead of having to manually add to service collection.
                .AddInjectableServices(_loggerFactory, _assemblyNames, _assemblyPaths)
                //
                //  Change what is returned to the user when an error occurs.
                .AddAlterRequestErrorObject(err =>
                {
                    err.Type = AboutErrorsUrl;
                    err.AddProperty("Some Custom Property",
                        new
                        {
                            SetInStartup = "Adding stuff like this MIGHT " +
                                           "be something you need for your standards."
                        }
                    );
                })
                //
                // Too much for this project, take a look at the Example Clean Architecture
                //.AddInvertedDependentsAndConfigureServices(
                //    _configuration,
                //    _loggerFactory,
                //    _environment.EnvironmentName,
                //    _assemblyNames,
                //    _assemblyPaths
                //)
#else
                // You may think to yourself... "This is no big deal, why would I need to do use your dumb InjectableServiceAttribute?
                // In fact I could clean up this bit of code just by putting it in an extension method and all is good."...
                //      Oh Really Tough guy? what happens when this presentation project does NOT have a reference to
                // the assembly that contains the implementation of the contract that lives at the application layer?
                //      huh!?
                //          then what!
                //                  huh!
                .AddSingleton<IRandomNumberGeneratorService, RandomNumberGeneratorService>()
#endif
                .AddMvc()
#if UseAPIBlox
                //
                // Handles cancellation token cancelled.
                .AddOperationCancelledExceptionFilter()
                //
                // Automatically fill in request object(s) from query params and route data.
                .AddPopulateRequestObjectActionFilter()
                //
                // Pagination
                .AddEnsurePaginationResultActionFilter(100)
                //
                // No pagination
                //.AddEnsureResponseResultActionFilter(data => new { NonPaginatedResources = data })
                //
                // Resource Validator.
                .AddValidateResourceActionFilter()
                //
                // If using AddMvcCore then we need this one.
                //.AddConsumesProducesJsonResourceResultFilters()
                //
                // Custom tokens, example has version
                .AddRouteTokensConvention(_configuration, _environment, "ExampleTokens")
#endif
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerExampleFeatures(SiteTitle, Version);
        }

        public void Configure(IApplicationBuilder app)
        {
#if UseAPIBlox
            //
            // Handle any and all server (500) errors with a defined structure.
            app.UseServerFaults();
            //
            // Good for testing how things respond (when things go too
            // quickly because your dev machine is such a monster!)
            app.UseSimulateWaitTime(_environment);
#else
            app.UseDeveloperExceptionPage();
#endif

            app.UseHsts();

            app.UseMvc();

            app.UseSwaggerExampleFeatures(SiteTitle, Version);
        }
    }
}
