using System.Collections.Generic;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Extensions;
using Examples.Resources;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    internal static class FullDynamicConfiguration
    {
        public static IMvcBuilder AddFullyDynamicConfiguration(this IMvcBuilder builder, bool production,
            out string dynamicControllersXmlFile
        )
        {
            var factory = new DynamicControllerFactory("ExampleDynamicControllersAssembly", production);

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


            factory.Compile(builder, @".\bin\debug\netcoreapp2.2", childDelete, childAll, childPut, childPost);

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
