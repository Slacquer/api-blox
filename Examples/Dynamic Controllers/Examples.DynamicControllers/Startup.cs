using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

#if UseAPIBlox
using APIBlox.NetCore.Types;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
#endif

//
//  This project is a clone of Examples.Features with additional bits for DynamicControllers.
//
namespace Examples
{
    internal class Startup
    {
#if UseAPIBlox
        private readonly string[] _assemblyNames;
        private readonly string[] _assemblyPaths;
        private readonly IHostingEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;
        private const string SiteTitle = "APIBlox Example: DynamiControllers";
#else
        private const string SiteTitle = "APIBlox Example: UseAPIBlox is OFF.";
#endif
        private const string Version = "v1";

#if UseAPIBlox
        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            _environment = environment;
            _loggerFactory = loggerFactory;

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
                //
                // Instead of having to manually add to service collection.
                .AddInjectableServices(_loggerFactory, _assemblyNames, _assemblyPaths)
#endif
                .AddMvc()
#if UseAPIBlox
                //
                //  DynamicControllers and configuration
                .AddDynamicControllersFeature(configs =>
                    configs.AddFamilyDynamicControllersConfiguration(),
                   addPostLocationHeaderResultFilter: true)
                //
                // Handles cancellation token cancelled.
                .AddOperationCancelledExceptionFilter()
                //
                // Fills in request objects for us.
                .AddPopulateGenericRequestObjectActionFilter()
                //
                // Pagination
                .AddEnsurePaginationResultActionFilter(100)
                //
                // Resource Validator.
                .AddValidateResourceActionFilter()
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
