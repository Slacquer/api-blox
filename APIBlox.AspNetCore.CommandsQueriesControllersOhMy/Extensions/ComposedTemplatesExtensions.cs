using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Enums;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;

namespace APIBlox.AspNetCore.Extensions
{
    /// <summary>
    ///     Class ComposedTemplatesExtensions.
    /// </summary>
    public static class ComposedTemplatesExtensions
    {
        private const string Prt = "[ProducesResponseType({0})]\n";
        private const string PrtResult = "[ProducesResponseType(typeof([RES_OBJECT_RESULT]), {0})]\n";

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for querying resources by some value(s).
        ///     <para>
        ///         Results are always returned in an <see cref="HandlerResponse" />. instance.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     When not set, response codes defaults to 200, 204, 401, 403.
        /// </remarks>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="templates">The templates.</param>
        /// <param name="options">The options.</param>
        /// <returns>IEnumerable&lt;IComposedTemplate&gt;.</returns>
        /// <exception cref="ArgumentException">
        ///     Must be a single object type. - TResponse
        ///     or
        ///     When providing status codes you must not use an empty list! - StatusCodes
        /// </exception>
        public static IEnumerable<IComposedTemplate> WriteQueryByController<TRequest, TResponse>(
            this IEnumerable<IComposedTemplate> templates, DynamicControllerTemplateOptions options
        )
            where TRequest : new()
        {
            if (typeof(TResponse).IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException("Must be a single object type.", nameof(TResponse));

            var codes = (options.StatusCodes ?? new List<int> {200, 204, 401, 403}).ToList();

            if (!codes.Any())
                throw new ArgumentException("When providing status codes you must not use an empty list!", nameof(options.StatusCodes));

            var (ac, cc) = GetComments(options, "Get a specific resource.");

            var action = Templates.GetDynamicAction("QueryBy", options.ActionRoute, ac);

            var template = new DynamicControllerComposedTemplate(options.NameSpace, options.ControllerRoute, action, cc);

            ((List<IComposedTemplate>) templates).Add(ParseReplaceAndAddToCollection(
                    template,
                    typeof(TRequest),
                    typeof(TResponse),
                    false,
                    req => options.ControllerName ?? $"QueryBy{req}Controller",
                    BuildResponseTypes(codes)
                )
            );

            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for querying for all resources.
        ///     <para>
        ///         Results are always returned in an <see cref="HandlerResponse" />. instance.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     When not set, response codes defaults to 200, 204, 401, 403.
        /// </remarks>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">The type of the t response</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="options">The controller template options.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        /// <exception cref="ArgumentException">Must be a enumerable object type. - TResponse</exception>
        public static IEnumerable<IComposedTemplate> WriteQueryAllController<TRequest, TResponse>(
            this IEnumerable<IComposedTemplate> templates, DynamicControllerTemplateOptions options
        )
            where TRequest : new()
            where TResponse : IEnumerable
        {
            if (!typeof(TResponse).IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException("Must be a enumerable object type.", nameof(TResponse));

            var codes = (options.StatusCodes ?? new List<int> {200, 204, 401, 403}).ToList();

            if (!codes.Any())
                throw new ArgumentException("When providing status codes you must not use an empty list!", nameof(options.StatusCodes));

            var (ac, cc) = GetComments(options, "Get all resources.");

            var action = Templates.GetDynamicAction("QueryAll", options.ActionRoute, ac);

            var template = new DynamicControllerComposedTemplate(options.NameSpace, options.ControllerRoute, action, cc);

            ((List<IComposedTemplate>) templates).Add(ParseReplaceAndAddToCollection(
                    template,
                    typeof(TRequest),
                    typeof(TResponse),
                    false,
                    req => options.ControllerName ?? $"QueryAll{req}Controller",
                    BuildResponseTypes(codes)
                )
            );

            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for deleting resources by some value.
        ///     <para>
        ///         Results are always returned in an <see cref="HandlerResponse" />. instance.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     When not set, response codes defaults to 204, 401, 403, 404.
        /// </remarks>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="options">The controller template options.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        public static IEnumerable<IComposedTemplate> WriteDeleteByController<TRequest>(
            this IEnumerable<IComposedTemplate> templates, DynamicControllerTemplateOptions options
        )
            where TRequest : new()
        {
            var codes = (options.StatusCodes ?? new List<int> {204, 401, 403, 404}).ToList();

            if (!codes.Any())
                throw new ArgumentException("When providing status codes you must not use an empty list!", nameof(options.StatusCodes));

            var (ac, cc) = GetComments(options, "Delete a specific resource.");

            var action = Templates.GetDynamicAction("DeleteBy", options.ActionRoute, ac);

            var template = new DynamicControllerComposedTemplate(options.NameSpace, options.ControllerRoute, action, cc);

            ((List<IComposedTemplate>) templates).Add(ParseReplaceAndAddToCollection(
                    template,
                    typeof(TRequest),
                    null,
                    false,
                    req => options.ControllerName ?? $"DeleteBy{req}Controller",
                    BuildResponseTypes(codes)
                )
            );
            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for updating a resources via PUT.
        ///     <para>
        ///         Results are always returned in an <see cref="HandlerResponse" />. instance.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     When not set, response codes defaults to 204, 401, 403, 404, 409.
        /// </remarks>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="options">The controller template options.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        public static IEnumerable<IComposedTemplate> WritePutController<TRequest>(
            this IEnumerable<IComposedTemplate> templates, DynamicControllerTemplateOptions options
        )
            where TRequest : new()
        {
            var codes = (options.StatusCodes ?? new List<int> {204, 400, 401, 403, 404, 409}).ToList();

            if (!codes.Any())
                throw new ArgumentException("When providing status codes you must not use an empty list!", nameof(options.StatusCodes));

            var (ac, cc) = GetComments(options, "Update a specific resource.");

            var action = Templates.GetDynamicAction("PutBy", options.ActionRoute, ac);

            var template = new DynamicControllerComposedTemplate(options.NameSpace, options.ControllerRoute, action, cc);

            ((List<IComposedTemplate>) templates).Add(ParseReplaceAndAddToCollection(
                    template,
                    typeof(TRequest),
                    null,
                    true,
                    req => options.ControllerName ?? $"PutBy{req}Controller",
                    BuildResponseTypes(codes)
                )
            );
            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for updating a resources via PATCH.
        ///     <para>
        ///         Results are always returned in an <see cref="HandlerResponse" />. instance.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     When not set, response codes defaults to 204, 401, 403, 404, 409.
        /// </remarks>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="options">The controller template options.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        public static IEnumerable<IComposedTemplate> WritePatchController<TRequest>(
            this IEnumerable<IComposedTemplate> templates, DynamicControllerTemplateOptions options
        )
            where TRequest : new()
        {
            var codes = (options.StatusCodes ?? new List<int> {204, 400, 401, 403, 404, 409}).ToList();

            if (!codes.Any())
                throw new ArgumentException("When providing status codes you must not use an empty list!", nameof(options.StatusCodes));

            var (ac, cc) = GetComments(options, "Update a specific resource.");

            var action = Templates.GetDynamicAction("PatchBy", options.ActionRoute, ac);

            var template = new DynamicControllerComposedTemplate(options.NameSpace, options.ControllerRoute, action, cc);

            ((List<IComposedTemplate>) templates).Add(ParseReplaceAndAddToCollection(
                    template,
                    typeof(TRequest),
                    null,
                    true,
                    req => options.ControllerName ?? $"PatchBy{req}Controller",
                    BuildResponseTypes(codes)
                )
            );
            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for creating a resources.
        ///     <para>
        ///         Results are always returned in an <see cref="HandlerResponse" />. instance.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     When not set, response codes defaults to 201, 204, 401, 403, 404, 409
        /// </remarks>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="options">The controller template options.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        /// <exception cref="ArgumentException">Must be a single object type. - TResponse</exception>
        public static IEnumerable<IComposedTemplate> WritePostController<TRequest, TResponse>(
            this IEnumerable<IComposedTemplate> templates, DynamicControllerTemplateOptions options
        )
            where TRequest : new()
        {
            if (typeof(TResponse).IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException("Must be a single object type.", nameof(TResponse));

            var codes = (options.StatusCodes ?? new List<int> {201, 204, 400, 401, 403, 404, 409}).ToList();

            if (!codes.Any())
                throw new ArgumentException("When providing status codes you must not use an empty list!", nameof(options.StatusCodes));

            var (ac, cc) = GetComments(options, "Create a new resource.");

            var action = Templates.GetDynamicAction("Post", options.ActionRoute, ac);

            var template = new DynamicControllerComposedTemplate(options.NameSpace, options.ControllerRoute, action, cc);

            ((List<IComposedTemplate>) templates).Add(ParseReplaceAndAddToCollection(
                    template,
                    typeof(TRequest),
                    typeof(TResponse),
                    true,
                    req => options.ControllerName ?? $"Post{req}Controller",
                    BuildResponseTypes(codes)
                )
            );

            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for creating a resources but NOT getting back an immediate
        ///     result.
        ///     <para>
        ///         Results are always returned in an <see cref="HandlerResponse" />. instance.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     When not set, response codes defaults to 202, 401, 403, 404, 409
        /// </remarks>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="options">The controller template options.</param>
        /// <exception cref="ArgumentException">Must be a single object type. - TResponse</exception>
        public static IEnumerable<IComposedTemplate> WritePostAcceptedController<TRequest>(
            this IEnumerable<IComposedTemplate> templates, DynamicControllerTemplateOptions options
        )
            where TRequest : new()
        {
            var codes = (options.StatusCodes ?? new List<int> {202, 400, 401, 403, 404, 409}).ToList();

            if (!codes.Any())
                throw new ArgumentException("When providing status codes you must not use an empty list!", nameof(options.StatusCodes));

            var (ac, cc) = GetComments(options, "Create a new resource.");

            var action = Templates.GetDynamicAction("PostAccepted", options.ActionRoute, ac);

            var template = new DynamicControllerComposedTemplate(options.NameSpace, options.ControllerRoute, action, cc);

            ((List<IComposedTemplate>) templates).Add(ParseReplaceAndAddToCollection(
                    template,
                    typeof(TRequest),
                    null,
                    true,
                    req => options.ControllerName ?? $"PostAccepted{req}Controller",
                    BuildResponseTypes(codes)
                )
            );

            return templates;
        }

        private static IComposedTemplate ParseReplaceAndAddToCollection(
            IComposedTemplate template,
            Type requestObj,
            Type responseObjectResult,
            bool requestObjMustHaveBody,
            Func<string, string> buildControllerName,
            (string, string) producesResponseTypes
        )
        {
            DynamicControllerFactory.ValidateRequestType(requestObj, requestObjMustHaveBody);

            if (!(responseObjectResult is null))
                DynamicControllerFactory.ValidateResponseType(responseObjectResult);

            var (reqObj, _, requestNs) = DynamicControllerFactory.WriteNameWithNamespaces(requestObj);
            var (parameters, paramNs) = DynamicControllerFactory.WriteInputParamsWithNamespaces(requestObj);
            var parameterComments = string.Join(Environment.NewLine, DynamicControllerFactory.WriteInputParamsXmlComments(requestObj));
            var reqObjComments = requestObj.GetSummary();
            var (resObj, realResObject, resultObjNs) = responseObjectResult is null
                ? ("", "", null)
                : DynamicControllerFactory.WriteNameWithNamespaces(responseObjectResult);
            var newReqObj = DynamicControllerFactory.WriteNewObject(requestObj);

            var ns = template.Action.Namespaces
                .Union(paramNs)
                .Union(requestNs);

            if (!(resultObjNs is null))
                ns = ns.Union(resultObjNs);

            template.Comments = template.Comments ?? reqObjComments;
            template.Action.Namespaces = ns.ToArray();

            var cn = buildControllerName(realResObject);

            template.Name = cn;

            template.Action.Tokens["[REQ_OBJECT]"] = reqObj;
            template.Action.Tokens["[RES_OBJECT_INNER_RESULT]"] = realResObject ?? resObj;
            template.Action.Tokens["[ACTION_COMMENTS]"] = template.Action.Comments ?? "";
            template.Action.Tokens["[ACTION_ROUTE]"] = template.Action.Route ?? "";
            template.Action.Tokens["[PARAMS_COMMENTS]"] = parameterComments;
            template.Action.Tokens["[RES_OBJECT_RESULT]"] = resObj;
            template.Action.Tokens["[ACTION_PARAMS]"] = $"{parameters},";
            template.Action.Tokens["[NEW_REQ_OBJECT]"] = newReqObj;
            template.Action.Tokens["[CONTROLLER_NAME]"] = cn;
            template.Action.Tokens["[RESPONSE_TYPES]"] = producesResponseTypes.Item1;
            template.Action.Tokens["[RESPONSE_TYPES_COMMENTS]"] = producesResponseTypes.Item2;

            template.Action.Compose();

            return template;
        }

        private static (DynamicComments, DynamicComments) GetComments(DynamicControllerTemplateOptions options, string summary)
        {
            var ac = new DynamicComments
            {
                Summary = options.ActionComments.Summary.IsEmptyNullOrWhiteSpace()
                    ? summary
                    : options.ActionComments.Summary,
                Remarks = options.ActionComments.Remarks.IsEmptyNullOrWhiteSpace()

                    //? @"![](https://github.com/Slacquer/api-blox/blob/master/logo-blue-small.png?raw=true) _Dynamic Controller Action_"
                    ? @"_Dynamic Controller Action_"
                    : options.ActionComments.Remarks
            };

            var cc = new DynamicComments
            {
                Summary = options.ControllerComments.Summary.IsEmptyNullOrWhiteSpace()
                    ? $"{options.ControllerName} endpoint(s)."
                    : options.ControllerComments.Summary
            };

            return (ac, cc);
        }

        private static (string, string) BuildResponseTypes(IEnumerable<int> statusCodes)
        {
            var sbCodes = new StringBuilder();
            var sbComments = new StringBuilder();

            foreach (var sc in statusCodes)
            {
                sbCodes.AppendFormat((CommonStatusCodes) sc == CommonStatusCodes.Status200Ok ||
                                     (CommonStatusCodes) sc == CommonStatusCodes.Status201Created
                        ? PrtResult
                        : Prt,
                    sc
                );

                if (Enum.IsDefined(typeof(CommonStatusCodes), sc))
                {
                    var statusCode = (CommonStatusCodes) sc;

                    var attr = statusCode.GetAttributeOfType<MetadataAttribute>();

                    var codeTitle = attr.V1.ToString();
                    var codeDesc = attr.V2.ToString();

                    sbComments.AppendLine($"/// <response code=\"{sc}\">{codeTitle}, {codeDesc}</response>");
                }
                else
                {
                    sbComments.AppendLine($"/// <response code=\"{sc}\">{sc}.</response>");
                }
            }

            return (sbCodes.ToString(), sbComments.ToString());
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        /// <summary>
        ///     Class Templates.
        /// </summary>
        private class Templates
        {
            /// <summary>
            ///     Gets the dynamic action.
            /// </summary>
            /// <param name="templatePath">The template path.</param>
            /// <param name="actionRoute">The action route.</param>
            /// <param name="comments">The action comments.</param>
            /// <returns>DynamicAction.</returns>
            public static DynamicAction GetDynamicAction(string templatePath, string actionRoute, DynamicComments comments)
            {
                var bits = EmbeddedResourceReader<Templates>.GetResources(templatePath);

                var methods = bits.ContainsKey("Methods")
                    ? bits["Methods"]
                    : null;

                return new DynamicAction(
                    templatePath,
                    actionRoute,
                    bits["ActionContent"],
                    bits["CtorArgs"],
                    bits["CtorBody"],
                    bits["Fields"].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries),
                    bits["Namespaces"].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries),
                    comments.ToString(),
                    methods
                );
            }
        }
    }
}
