using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Extensions;
using APIBlox.NetCore.Types;
using Examples.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

//
//  This project is a clone of Examples.Features with additional bits for DynamicControllers.
//
namespace Examples
{
    internal class Startup
    {
        private const string dll =
            @"D:\\Source\\Repos\\FKS\\api-blox\\SlnTests\\bin\\Debug\\netcoreapp2.2\\SuccessfullyCompileMultipleControllersAndAssemblyExists\\OutputFile.dll";

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

                //.AddApplicationPart(Assembly.LoadFile(dll))

                //
                //  DynamicControllers and configuration
                //.AddDynamicControllersFeature(configs =>
                //    {
                //        //configs.AddFamilyDynamicControllersConfiguration();
                //        configs.AddFullyDynamicConfiguration();
                //    },
                //    addPostLocationHeaderResultFilter: true
                //)
                .AddDynamicControllers()

                //
                // Handles cancellation token cancelled.
                .AddOperationCancelledExceptionFilter()

                //
                // Fills in request objects for us.
                .AddPopulateGenericRequestObjectActionFilter()

                //
                // Pagination
                //.AddEnsurePaginationResultActionFilter(100)
                .AddEnsureResponseResultActionFilter()

                //
                // Resource Validator.
                .AddValidateResourceActionFilter()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerExampleFeatures(SiteTitle, Version);
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

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class MvcBuilderExtensions.
    /// </summary>
    public static class MvcBuilderExtensionsDynamicControllers
    {
        public static IMvcBuilder AddDynamicControllers(this IMvcBuilder builder)
        {
            var factory = new DynamicControllerFactory("ExampleDynamicControllersAssembly", true);

            var childAll = factory.WriteQueryAllController<ChildrenRequest, IEnumerable<ChildResponse>>(
                null,
                "DynamicControllers",
                "Children",
                "api/[controller]/{someRouteValueWeNeed}/parents/{parentId}/children"
            );

            var childDelete = factory.WriteDeleteByController<ChildByIdRequest>(
                "{childId}",
                "DynamicControllers",
                "Children",
                "api/[controller]/{someRouteValueWeNeed}/parents/{parentId}/children"
            );

            var childPut = factory.WritePutController<ChildPutRequest>(
                "{childId}",
                "DynamicControllers",
                "Children",
                "api/[controller]/{someRouteValueWeNeed}/parents/{parentId}/children"
            );

            var childPost = factory.WritePostController<ChildPostRequest, ChildResponse>(
                null,
                "DynamicControllers",
                "Children",
                "api/[controller]/{someRouteValueWeNeed}/parents/{parentId}/children"
            );

            var ass = factory.Compile(@".\FullyDynamic", childDelete,childAll, childPut,childPost);

            if (ass is null || factory.CompilationErrors != null)
                throw new Exception(factory.CompilationErrors.First());

            builder.ConfigureApplicationPartManager(pm =>
                {
                    var part = new AssemblyPart(Assembly.LoadFrom(ass.FullName));
                    pm.ApplicationParts.Add(part);
                }
            );
            
            return builder;
        }
    }
}
