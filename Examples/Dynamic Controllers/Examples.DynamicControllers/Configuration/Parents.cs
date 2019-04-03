#region -    Using Statements    -

using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Extensions;
using Examples.Resources;

#endregion

namespace Examples.Configuration
{
    internal static class Parents
    {
        public static List<IComposedTemplate> AddParentsControllerTemplates(this List<IComposedTemplate> templates)
        {
            const string nameSpace = "Examples";
            const string controllerRoute = "api/[controller]/parents";

            templates
                .WriteQueryByController<ParentRequest, ParentResponse>(
                    "{parentId}",
                    nameSpace,
                    "Parents",
                    controllerRoute
                );

            return templates;
        }
    }
}
