using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;

namespace APIBlox.AspNetCore.Extensions
{
    /// <summary>
    ///     Class ComposedTemplatesExtensions.
    /// </summary>
    public static class ComposedTemplatesExtensions
    {
        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for querying resources by some value.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="actionRoute">The action route.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        /// <exception cref="ArgumentException">
        ///     QueryBy requires a route for the action, maybe something like {id}. - actionRoute
        ///     or
        ///     Must be a single object type. - TResponse
        /// </exception>
        public static IEnumerable<IComposedTemplate> WriteQueryByController<TRequest, TResponse>(
            this IEnumerable<IComposedTemplate> templates,
            string actionRoute,
            string controllerName = null,
            string controllerRoute = "api/[controller]",
            string nameSpace = "DynamicControllers"
        )
            where TRequest : new()
        {
            if (actionRoute.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException("QueryBy requires a route for the action, maybe something like {id}.",
                    nameof(actionRoute)
                );

            if (typeof(TResponse).IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException("Must be a single object type.", nameof(TResponse));

            var action = Templates.GetDynamicAction("QueryBy", actionRoute);

            var template = new DynamicControllerComposedTemplate(nameSpace, controllerRoute, action);

            ((List<IComposedTemplate>)templates).Add(ParseReplaceAndAddToCollection(
                 template,
                 typeof(TRequest),
                 typeof(TResponse),
                 false,
                 req => controllerName.ToPascalCase() ?? $"QueryBy{req}Controller"
             ));

            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for querying for all resources.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="actionRoute">The action route.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        /// <exception cref="ArgumentException">Must be a enumerable object type. - TResponse</exception>
        public static IEnumerable<IComposedTemplate> WriteQueryAllController<TRequest, TResponse>(
            this IEnumerable<IComposedTemplate> templates,
            string actionRoute = null,
            string controllerName = null,
            string controllerRoute = "api/[controller]",
            string nameSpace = "DynamicControllers"
        )
            where TRequest : new()
            where TResponse : IEnumerable
        {
            if (!typeof(TResponse).IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException("Must be a enumerable object type.", nameof(TResponse));

            var action = Templates.GetDynamicAction("QueryAll", actionRoute);

            var template = new DynamicControllerComposedTemplate(nameSpace, controllerRoute, action);

            ((List<IComposedTemplate>)templates).Add(ParseReplaceAndAddToCollection(
                template,
                typeof(TRequest),
                typeof(TResponse),
                false,
                req => controllerName.ToPascalCase() ?? $"QueryAll{req}Controller"
            ));

            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for deleting resources by some value.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="actionRoute">The action route.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        public static IEnumerable<IComposedTemplate> WriteDeleteByController<TRequest>(
            this IEnumerable<IComposedTemplate> templates,
            string actionRoute = null,
            string controllerName = null,
            string controllerRoute = "api/[controller]",
            string nameSpace = "DynamicControllers"
        )
            where TRequest : new()
        {
            var action = Templates.GetDynamicAction("DeleteBy", actionRoute);

            var template = new DynamicControllerComposedTemplate(nameSpace, controllerRoute, action);

            ((List<IComposedTemplate>)templates).Add(ParseReplaceAndAddToCollection(
                template,
                typeof(TRequest),
                null,
                false,
                req => controllerName.ToPascalCase() ?? $"DeleteBy{req}Controller"
            ));
            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for updating a resources via PUT.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="actionRoute">The action route.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        public static IEnumerable<IComposedTemplate> WritePutController<TRequest>(
            this IEnumerable<IComposedTemplate> templates,
            string actionRoute = null,
            string controllerName = null,
            string controllerRoute = "api/[controller]",
            string nameSpace = "DynamicControllers"
        )
            where TRequest : new()
        {
            var action = Templates.GetDynamicAction("PutBy", actionRoute);

            var template = new DynamicControllerComposedTemplate(nameSpace, controllerRoute, action);

            ((List<IComposedTemplate>)templates).Add(ParseReplaceAndAddToCollection(
                 template,
                 typeof(TRequest),
                 null,
                 true,
                 req => controllerName.ToPascalCase() ?? $"PutBy{req}Controller"
             ));
            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for updating a resources via PATCH.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="actionRoute">The action route.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        public static IEnumerable<IComposedTemplate> WritePatchController<TRequest>(
            this IEnumerable<IComposedTemplate> templates,
            string actionRoute = null,
            string controllerName = null,
            string controllerRoute = "api/[controller]",
            string nameSpace = "DynamicControllers"
        )
            where TRequest : new()
        {
            var action = Templates.GetDynamicAction("PatchBy", actionRoute);

            var template = new DynamicControllerComposedTemplate(nameSpace, controllerRoute, action);

            ((List<IComposedTemplate>)templates).Add(ParseReplaceAndAddToCollection(
                 template,
                 typeof(TRequest),
                 null,
                 true,
                 req => controllerName.ToPascalCase() ?? $"PatchBy{req}Controller"
             ));
            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for creating a resources.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="actionRoute">The action route.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        /// <exception cref="ArgumentException">Must be a single object type. - TResponse</exception>
        public static IEnumerable<IComposedTemplate> WritePostController<TRequest, TResponse>(
            this IEnumerable<IComposedTemplate> templates,
            string actionRoute = null,
            string controllerName = null,
            string controllerRoute = "api/[controller]",
            string nameSpace = "DynamicControllers"
        )
            where TRequest : new()
        {
            if (typeof(TResponse).IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException("Must be a single object type.", nameof(TResponse));

            var action = Templates.GetDynamicAction("Post", actionRoute);

            var template = new DynamicControllerComposedTemplate(nameSpace, controllerRoute, action);

            ((List<IComposedTemplate>)templates).Add(ParseReplaceAndAddToCollection(
                   template,
                   typeof(TRequest),
                   typeof(TResponse),
                   true,
                   req => controllerName.ToPascalCase() ?? $"Post{req}Controller"
               ));

            return templates;
        }

        /// <summary>
        ///     Adds a <see cref="DynamicControllerComposedTemplate" /> for creating a resources but NOT getting back an
        ///     immediate result.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="templates">Current list of templates.</param>
        /// <param name="actionRoute">The action route.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        /// <exception cref="ArgumentException">Must be a single object type. - TResponse</exception>
        public static IEnumerable<IComposedTemplate> WritePostAcceptedController<TRequest>(
            this IEnumerable<IComposedTemplate> templates,
            string actionRoute = null,
            string nameSpace = "DynamicControllers",
            string controllerName = null,
            string controllerRoute = "api/[controller]"
        )
            where TRequest : new()
        {
            var action = Templates.GetDynamicAction("PostAccepted", actionRoute);

            var template = new DynamicControllerComposedTemplate(nameSpace, controllerRoute, action);

            ((List<IComposedTemplate>)templates).Add(ParseReplaceAndAddToCollection(
                   template,
                   typeof(TRequest),
                   null,
                   true,
                   req => controllerName.ToPascalCase() ?? $"PostAccepted{req}Controller"
               ));

            return templates;
        }

        private static IComposedTemplate ParseReplaceAndAddToCollection(
            IComposedTemplate template,
            Type requestObj,
            Type responseObjectResult,
            bool requestObjMustHaveBody,
            Func<string, string> buildControllerName
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

            template.Comments = reqObjComments;
            template.Action.Namespaces = ns.ToArray();

            var cn = buildControllerName(realResObject);

            template.Name = cn;

            template.Action.Tokens["[REQ_OBJECT]"] = reqObj;
            template.Action.Tokens["[RES_OBJECT_INNER_RESULT]"] = realResObject ?? resObj;
            template.Action.Tokens["[ACTION_ROUTE]"] = template.Action.Route ?? "";
            template.Action.Tokens["[PARAMS_COMMENTS]"] = parameterComments;
            template.Action.Tokens["[RES_OBJECT_RESULT]"] = resObj;
            template.Action.Tokens["[ACTION_PARAMS]"] = $"{parameters},";
            template.Action.Tokens["[NEW_REQ_OBJECT]"] = newReqObj;
            template.Action.Tokens["[CONTROLLER_NAME]"] = cn;

            template.Action.Compose();

            return template;
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
            /// <returns>DynamicAction.</returns>
            public static DynamicAction GetDynamicAction(string templatePath, string actionRoute)
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
                    methods
                );
            }
        }
    }
}
