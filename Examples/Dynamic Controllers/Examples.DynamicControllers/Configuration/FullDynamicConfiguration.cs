﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Extensions;
using Examples.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    internal static class FullDynamicConfiguration
    {
        public static IMvcBuilder AddFullyDynamicConfiguration(this IMvcBuilder builder, ILoggerFactory loggerFactory, IHostingEnvironment environment,
            out string dynamicControllersXmlFile
        )
        {
            var factory = new DynamicControllerFactory(loggerFactory, "ExampleDynamicControllersAssembly", environment.IsProduction());

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

            var childPatch = factory.WritePatchController<ChildPatchRequest>(
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

            //var childPost = factory.WritePostController<ChildPostRequest, ChildResponse>(
            //    null,
            //    nameSpace,
            //    "Children",
            //    controllerRoute
            //);

            var childPostAccepted = factory.WritePostAcceptedController<ChildPostRequest>(
                null,
                nameSpace,
                "Children",
                controllerRoute
            );

            var output = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            factory.Compile(builder,
                output,
                environment.IsProduction(),
                childById,
                childDelete,
                childAll,
                childPut,
                childPatch,
                childPostAccepted
            );

            var (_, _, xml) = factory.OutputFiles;

            dynamicControllersXmlFile = xml;

            return builder;
        }
    }
}
