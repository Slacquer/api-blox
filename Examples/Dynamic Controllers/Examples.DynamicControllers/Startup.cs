using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using APIBlox.NetCore.Types;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Extensions;
using APIBlox.AspNetCore.Types;
using Examples.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

//
//  This project is a clone of Examples.Features with additional bits for DynamicControllers.
//
namespace Examples
{
    internal class Startup
    {
        private readonly string[] _assemblyNames;
        private readonly string[] _assemblyPaths;
        private readonly IHostingEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;
        private const string SiteTitle = "APIBlox Example: DynamiControllers";
        private const string Version = "v1";

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

        private const string dll =
            @"D:\\Source\\Repos\\FKS\\api-blox\\SlnTests\\bin\\Debug\\netcoreapp2.2\\SuccessfullyCompileMultipleControllersAndAssemblyExists\\OutputFile.dll";

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


            var factory = new DynamicControllerFactory("ExampleDynamicControllersAssembly", false);

            var c1 = factory.WriteQueryByController<ByIdRequest, DynamicControllerResponse>(
                "dynamicControllerResponses/{someId}",
                "DynamicControllers",
                  "MyDynamicController"
            );

            var c2 = factory.WriteQueryAllController<AllRequest, IEnumerable<DynamicControllerResponse>>(
                "DynamicControllers",
                "MyDynamicController",actionRoute:"dynamicControllerResponses"
            );


            var ass = factory.Compile(@".\FullyDynamic", c1, c2);


            if (ass is null || factory.CompilationErrors != null)
                throw new System.Exception(factory.CompilationErrors.First());

            builder.ConfigureApplicationPartManager(pm =>
                {
                    var part = new AssemblyPart(Assembly.LoadFrom(ass.FullName));
                    pm.ApplicationParts.Add(part);
                }
            );


            //if (fi is null || factory.CompilationErrors != null)
            //    throw new System.Exception(factory.CompilationErrors.First());


            //var fi = factory.Compile(outfile, c1, c2, c2b, c2c, c2d, c2e, c3);
            //var fi = factory.Compile(outfile, c2e);

            //var ass = factory.Compile(c1, c2, c2b, c2c, c2d, c2e, c3);

            //var c2 = factory.WriteQueryAllController<ChildrenRequest, IEnumerable<ChildResponse>>(
            //     nameSpace: "Examples"
            //);

            //var c2b = factory.WriteQueryByController<ChildByIdRequest, ChildResponse>(
            //     nameSpace: "Examples", controllerRoute: "api/[controller]/{someRouteValueWeNeed}/parents/{parentId}/children", actionRoute: "{childId}"
            //);

            //var c2c = factory.WriteDeleteByController<ChildByIdRequest>(
            //     nameSpace: "Examples", controllerRoute: "api/[controller]/{someRouteValueWeNeed}/parents/{parentId}/children", actionRoute: "{childId}"
            //);

            //var c2d = factory.WritePutController<ChildPutRequest>(
            //    nameSpace: "Examples", controllerRoute: "api/[controller]/parents/{parentId}/children", actionRoute: "{childId}"
            //);

            //var c2e = factory.WritePostController<ChildPostRequest, ChildResponse>(
            //     nameSpace: "Examples", controllerRoute: "api/[controller]/parents/{parentId}/children"
            //);

            //var c3 = factory.WriteQueryAllController<ParentRequest, IEnumerable<ParentResponse>>(
            //     controllerName: "Parents", nameSpace: "Examples"
            //);

            //builder.ConfigureApplicationPartManager(pm =>
            //    {
            //        var part = new AssemblyPart(ass);
            //        pm.ApplicationParts.Add(part);
            //    }
            //);

            return builder;
        }
    }
}