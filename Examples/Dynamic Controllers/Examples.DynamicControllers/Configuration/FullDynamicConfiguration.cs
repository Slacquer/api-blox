using System.Collections.Generic;
using System.IO;
using System.Reflection;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Exceptions;
using APIBlox.AspNetCore.Extensions;
using Examples;
using Examples.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
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
            builder.AddComposeControllers(loggerFactory, environment,
                (f, t) =>
                {
                    const string nameSpace = "Examples";
                    const string controllerRoute = "api/[controller]/parents/{parentId}/children";

                    t.Add(f.WriteQueryByController<ChildByIdRequest, ChildResponse>(
                            "{childId}", nameSpace, "Children", controllerRoute)
                    );

                    t.Add(f.WritePutController<ChildPutRequest>(
                        "{childId}", nameSpace, "Children", controllerRoute)
                    );

                    t.Add(f.WritePatchController<ChildPatchRequest>(
                        "{childId}", nameSpace, "Children", controllerRoute)
                    );

                    t.Add(f.WriteDeleteByController<ChildByIdRequest>(
                        "{childId}", nameSpace, "Children", controllerRoute)
                    );

                    t.Add(f.WriteQueryAllController<ChildrenRequest, IEnumerable<ChildResponse>>(
                        null, nameSpace, "Children", controllerRoute)
                    );

                    //t.Add(f.WritePostController<ChildPostRequest, ChildResponse>(
                    //    null, nameSpace, "Children", controllerRoute)
                    //);

                    t.Add(f.WritePostAcceptedController<ChildPostRequest>(
                        null, nameSpace, "Children", controllerRoute)
                    );

                    f.AdditionalAssemblyReferences.Add(Assembly.GetAssembly(typeof(Startup)));

                }, out dynamicControllersXmlFile);

            return builder;
        }
    }
}
