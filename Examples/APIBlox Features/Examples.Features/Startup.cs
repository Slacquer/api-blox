
using Examples.Controllers;

#if UseAPIBlox
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using APIBlox.NetCore.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

#else
using Examples.Contracts;
using Examples.Services;
#endif

namespace Examples
{
    internal class Startup
    {
        private const string SiteTitle = "APIBlox Example: Features";
        private const string Version = "v1";

    #if UseAPIBlox
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;

            _assemblyNames = new[]
            {
                "Examples."
            };

            var excludeThese = PathParser.FindAllSubDirectories($"{environment.ContentRootPath}\\**\\obj")
                .Select(di => $"!{di.FullName}");

            _assemblyPaths = new List<string>(excludeThese)
            {
                _environment.ContentRootPath,
                new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName
            }.ToArray();
        }
    #endif

        public void ConfigureServices(IServiceCollection services)
        {
            services
            #if UseAPIBlox
                .AddServerFaults()

                //
                // Instead of having to manually add to service collection.
                .AddInjectableServices(Program.StartupLogger, _assemblyNames, _assemblyPaths)

                //
                //  Change what is returned to the user when an error occurs.
                .AddAlterRequestErrorObject(err =>
                    {
                        err.Type = AboutErrorsUrl;
                        err.AddProperty("Stack-trace",
                            Environment.StackTrace
                        );
                    }
                )

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
                .AddCamelCaseResultsOptions()

                //
                // Handles cancellation token cancelled.
                .AddOperationCancelledExceptionFilter()

                //
                // Pagination
                .AddEnsurePaginationResultActionFilter(Program.StartupLogger,
                    onlyForThesePaths:new List<string>{ "/DevApi/versions/2/resources/Examples"})

                .AddPaginationResultForPath("/DevApi/versions/1/resources/Examples")

                //
                // No pagination
                //.AddEnsureResponseResultActionFilter(_loggerFactory,false, defineResponseFunc: data => new { NonPaginatedResources = data })

                //
                // Resource Validator.
                .AddValidateResourceActionFilter()

                //
                // Custom tokens, example has version
                .AddRouteTokensConvention(_configuration, _environment, "ExampleTokens")

                //
                // IQuery maps stuff (allowing alternate names to be used)
                .AddFromQueryWithAlternateNamesBinder()

            #endif
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            services.AddSwaggerExampleFeatures(SiteTitle, Version);
        }

        public void Configure(IApplicationBuilder app)
        {
        #if UseAPIBlox

            //
            // Handle any and all server (500) errors with a defined structure.
            app.UseServerFaults(requestErrorObjectAction: o => o.AddProperty("stack-trace", Environment.StackTrace));

            //
            // Good for testing how things respond (when things go too
            // quickly because your dev machine is such a monster!)
            app.UseSimulateWaitTime(_environment);
        #else
            //app.UseDeveloperExceptionPage();
        #endif
            app.UseHsts();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerExampleFeatures(SiteTitle, Version);
        }
    #if UseAPIBlox
        private readonly string[] _assemblyNames;
        private readonly string[] _assemblyPaths;
        private const string AboutErrorsUrl = "http://hey.look.at.me/errorcodes";
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
    #endif
    }
}
