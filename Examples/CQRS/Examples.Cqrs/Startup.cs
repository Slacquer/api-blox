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
        private const string SiteTitle = "APIBlox Example: CQRS";
        private const string Version = "v1";
        private readonly string[] _assemblyNames;
        private readonly string[] _assemblyPaths;

        public Startup(IWebHostEnvironment environment)
        {
            _assemblyNames = new[]
            {
                "Examples."
            };

            var excludeThese = PathParser.FindAllSubDirectories($"{environment.ContentRootPath}\\**\\obj")
                .Select(di => $"!{di.FullName}");

            _assemblyPaths = new List<string>(excludeThese)
            {
                environment.ContentRootPath,
                new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName
            }.ToArray();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddServerFaults()

                //
                // Instead of having to manually add to service collection.
                .AddInjectableServices(Program.StartupLogger, _assemblyNames, _assemblyPaths)

                //
                //  Some decorators, be sure to do AFTER injectable services have been established.
                .AddCqrsDecorators(Program.StartupLogger)
                .AddMvc()

                //
                // Handles cancellation token cancelled.
                .AddOperationCancelledExceptionFilter()

                //
                // Pagination
                //.AddEnsurePaginationResultActionFilter(_loggerFactory, defaultPageSize: 100)
                .AddEnsureResponseResultActionFilter(Program.StartupLogger)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSwaggerExampleFeatures(SiteTitle, Version);
        }

        public void Configure(IApplicationBuilder app)
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

            app.UseSwaggerExampleFeatures(SiteTitle, Version);
        }
    }
}
