using System.Collections.Generic;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Extensions;
using Examples.Resources;
using FluentValidation;

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
                )
                .WritePutController<ChildPutRequest>(
                    "{childId}",
                    nameSpace,
                    "Children",
                    controllerRoute
                )
                .WritePatchController<ChildPatchRequest>(
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

                //.WritePostController<ChildPostRequest, ChildResponse>(
                //     null,
                //     nameSpace,
                //     "Children",
                //     controllerRoute
                // )
                .WritePostAcceptedController<ChildPostRequest>(
                    null,
                    nameSpace,
                    "Children",
                    controllerRoute
                )
                ;

            return templates;
        }
    }

    /// <summary>
    ///     Class ChildPutRequestValidator.
    /// </summary>
    public class ChildPutRequestValidator : AbstractValidator<PersonModel>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChildPutRequestValidator" /> class.
        /// </summary>
        public ChildPutRequestValidator()
        {
            RuleFor(p => p.FirstName).NotEmpty();
        }
    }
}
