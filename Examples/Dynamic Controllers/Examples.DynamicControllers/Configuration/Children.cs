using System.Collections.Generic;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Extensions;
using APIBlox.AspNetCore.Types;
using Examples.Resources;
using FluentValidation;

namespace Examples.Configuration
{
    internal static class Children
    {
        public static List<IComposedTemplate> AddChildrenControllerTemplates(this List<IComposedTemplate> templates)
        {
            var options = new DynamicControllerTemplateOptions
            {
                ControllerRoute = "api/[controller]/parents/{parentId}/children",
                NameSpace = "Examples",
                ControllerName = "Children",
                ControllerComments = new DynamicComments
                {
                    Summary = "# Kids are a PITA!",
                    Remarks = " # We love them anyways!" // Useless in swashbuckle (it would seem).
                }
            };

            templates
                .WriteQueryByController<ChildByIdRequest, ChildResponse>(options.Set(p => p.ActionRoute = "{childId}"))
                .WritePutController<ChildPutRequest>(options)
                .WritePatchController<ChildPatchRequest>(options)
                .WriteDeleteByController<ChildByIdRequest>(options)

                //.WritePostController<ChildPostRequest, ChildResponse>(options.Set(p => p.ActionRoute = null, p => p.StatusCodes = null))
                .WritePostAcceptedController<ChildPostRequest>(options.Set(p => p.ActionRoute = null, p => p.StatusCodes = null))
                .WriteQueryAllController<ChildrenRequest, IEnumerable<ChildResponse>>(options.Set(p => p.ActionRoute = null, p => p.StatusCodes = new List<int> { 200, 401, 403 }))
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
