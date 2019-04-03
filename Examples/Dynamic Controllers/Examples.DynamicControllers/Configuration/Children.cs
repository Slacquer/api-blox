#region -    Using Statements    -

using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Extensions;
using Examples.Resources;

#endregion

namespace Examples.Configuration
{
    internal static class Children
    {
        public static List<IComposedTemplate> AddChildrenControllerTemplates(this List<IComposedTemplate> templates)
        {
            const string nameSpace = "Examples";
            const string controllerRoute = "api/[controller]/parents/{parentId}/children";

            templates
                .WriteQueryByController<ChildByIdRequest, ChildResponse>(
                    "{childId}",
                    nameSpace,
                    "Children",
                    controllerRoute
                ).WritePutController<ChildPutRequest>(
                    "{childId}",
                    nameSpace,
                    "Children",
                    controllerRoute
                ).WritePatchController<ChildPatchRequest>(
                    "{childId}",
                    nameSpace,
                    "Children",
                    controllerRoute
                ).WriteDeleteByController<ChildByIdRequest>(
                    "{childId}",
                    nameSpace,
                    "Children",
                    controllerRoute
                ).WriteQueryAllController<ChildrenRequest, IEnumerable<ChildResponse>>(
                    null,
                    nameSpace,
                    "Children",
                    controllerRoute
                )

                //childTemplates.WritePostController<ChildPostRequest, ChildResponse>(
                //    null,
                //    nameSpace,
                //    "Children",
                //    controllerRoute
                //);
                .WritePostAcceptedController<ChildPostRequest>(
                    null,
                    nameSpace,
                    "Children",
                    controllerRoute
                );

            return templates;
        }
    }
}
