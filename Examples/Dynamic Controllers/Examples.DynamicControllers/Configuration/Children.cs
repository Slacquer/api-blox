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
                    "Children",
                    controllerRoute,
                     nameSpace
                )
                .WritePutController<ChildPutRequest>(
                    "{childId}",
                    "Children",
                    controllerRoute,
                     nameSpace
                )
                .WritePatchController<ChildPatchRequest>(
                    "{childId}",
                    "Children",
                    controllerRoute,
                     nameSpace
                ).WriteDeleteByController<ChildByIdRequest>(
                    "{childId}",
                    "Children",
                    controllerRoute,
                     nameSpace
                ).WriteQueryAllController<ChildrenRequest, IEnumerable<ChildResponse>>(
                    null,
                    "Children",
                    controllerRoute,
                     nameSpace, new List<int> { 200, 401, 403 }
                )

                //.WritePostController<ChildPostRequest, ChildResponse>(
                //        null,
                //        "Children",
                //        controllerRoute,
                //        nameSpace
                // )

                .WritePostAcceptedController<ChildPostRequest>(
                    null,
                    "Children",
                    controllerRoute,
                    nameSpace
                );

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

    /// <inheritdoc />
    /// <summary>
    ///     Class ChildPByIdRequestValidator.
    /// </summary>
    public class ChildPByIdRequestValidator : AbstractValidator<ChildByIdRequest>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChildPutRequestValidator" /> class.
        /// </summary>
        public ChildPByIdRequestValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
            RuleFor(p => p.Help).NotEmpty();
        }
    }
}
