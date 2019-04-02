using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using APIBlox.NetCore.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Examples
{
    internal class Startup
    {
        private const string SiteTitle = "APIBlox Example: DynamiControllers";
        private const string Version = "v1";
        private readonly string[] _assemblyNames;
        private readonly string[] _assemblyPaths;
        private readonly IHostingEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            _environment = environment;
            _loggerFactory = loggerFactory;

            _assemblyNames = new[]
            {
                "Examples."
            };

            var excludeThese = PathParser.FindAllSubDirectories($"{environment.ContentRootPath}\\**\\obj")
                .Select(di => $"!{di.FullName}").ToList();

            _assemblyPaths = new List<string>(excludeThese)
            {
                _environment.ContentRootPath,
                new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName
            }.ToArray();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddServerFaults()

                //
                // Instead of having to manually add to service collection.
                .AddInjectableServices(_loggerFactory, _assemblyNames, _assemblyPaths)
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)

                //
                //  DynamicControllers and configuration
                .AddFullyDynamicConfiguration(_environment, out var dynamicControllersXmlFile)

                //
                // Handles cancellation token cancelled.
                .AddOperationCancelledExceptionFilter()

                //
                // Pagination
                //.AddEnsurePaginationResultActionFilter(100)
                .AddEnsureResponseResultActionFilter()

                //
                // Resource Validator.
                .AddValidateResourceActionFilter()

                //
                // Make sure all results are camel cased.
                .AddCamelCaseResultsOptions();

            services.AddSwaggerExampleFeatures(SiteTitle, Version, dynamicControllersXmlFile);
        }

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