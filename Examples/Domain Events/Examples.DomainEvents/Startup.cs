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
        private const string SiteTitle = "APIBlox Example: Domain Events";
        private const string Version = "v1";
        private readonly string[] _assemblyNames;
        private readonly string[] _assemblyPaths;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

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
                //
                // Instead of having to manually add to service collection.
                .AddInjectableServices(_loggerFactory, _assemblyNames, _assemblyPaths)
                
                .AddMvc()
                
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerExampleFeatures(SiteTitle, Version);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHsts();

            app.UseMvc();

            app.UseSwaggerExampleFeatures(SiteTitle, Version);
        }
    }
}
