using System.Collections.Generic;
using System.IO;
using System.Reflection;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Extensions;
using Examples.Resources;
using Microsoft.AspNetCore.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    internal static class FullDynamicConfiguration
    {
        public static IMvcBuilder AddFullyDynamicConfiguration(this IMvcBuilder builder, IHostingEnvironment environment,
            out string dynamicControllersXmlFile
        )
        {
            var factory = new DynamicControllerFactory("ExampleDynamicControllersAssembly", environment.IsProduction());

            const string nameSpace = "Examples";
            const string controllerRoute = "api/[controller]/{someRouteValueWeNeed}/parents/{parentId}/children";

            var childById = factory.WriteQueryByController<ChildByIdRequest, ChildResponse>(
                "{childId}",
                nameSpace,
                "Children",
                controllerRoute
            );

            var childPut = factory.WritePutController<ChildPutRequest>(
                "{childId}",
                nameSpace,
                "Children",
                controllerRoute
            );

            var childDelete = factory.WriteDeleteByController<ChildByIdRequest>(
                "{childId}",
                nameSpace,
                "Children",
                controllerRoute
            );


            var childAll = factory.WriteQueryAllController<ChildrenRequest, IEnumerable<ChildResponse>>(
                null,
                nameSpace,
                "Children",
                controllerRoute
            );

            var childPost = factory.WritePostController<ChildPostRequest, ChildResponse>(
                null,
                nameSpace,
                "Children",
                controllerRoute
            );

            var output = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            factory.Compile(builder, output, childById, childDelete, childAll, childPut, childPost);

            if (!(factory.CompilationErrors is null) || !(factory.CompilationWarnings is null))
            {
                dynamicControllersXmlFile = null;

                return builder;
            }

            var (_, _, xml) = factory.OutputFiles;

            dynamicControllersXmlFile = xml;

            return builder;
        }
    }
}
