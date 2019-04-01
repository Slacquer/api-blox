using System;
using System.Collections;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;

namespace APIBlox.AspNetCore.Extensions
{
    /// <summary>
    ///     Class DynamicControllerFactoryExtensions.
    /// </summary>
    public static class DynamicControllerFactoryExtensions
    {
        /// <summary>
        ///     Creates a <see cref="DynamicControllerComposedTemplate"/> for querying resources by some value.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="factory">The factory.</param>
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
        public static DynamicControllerComposedTemplate WriteQueryByController<TRequest, TResponse>(
            this DynamicControllerFactory factory,
            string actionRoute,
            string nameSpace = "DynamicControllers",
            string controllerName = null,
            string controllerRoute = "api/[controller]"
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

            ParseAndReplace(factory,
                template,
                typeof(TRequest),
                typeof(TResponse),
                false,
                req => controllerName.ToPascalCase() ?? $"QueryBy{req}Controller"
            );

            return template;
        }

        /// <summary>
        ///     Creates a <see cref="DynamicControllerComposedTemplate"/> for querying for all resources.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="actionRoute">The action route.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        /// <exception cref="ArgumentException">Must be a enumerable object type. - TResponse</exception>
        public static DynamicControllerComposedTemplate WriteQueryAllController<TRequest, TResponse>(
            this DynamicControllerFactory factory,
            string actionRoute = null,
            string nameSpace = "DynamicControllers",
            string controllerName = null,
            string controllerRoute = "api/[controller]"
        )
            where TRequest : new()
            where TResponse : IEnumerable
        {
            if (!typeof(TResponse).IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException("Must be a enumerable object type.", nameof(TResponse));

            var action = Templates.GetDynamicAction("QueryAll", actionRoute);

            var template = new DynamicControllerComposedTemplate(nameSpace, controllerRoute, action);

            ParseAndReplace(factory,
                template,
                typeof(TRequest),
                typeof(TResponse),
                false,
                req => controllerName.ToPascalCase() ?? $"QueryAll{req}Controller"
            );

            return template;
        }

        /// <summary>
        ///     Creates a <see cref="DynamicControllerComposedTemplate"/> for deleting resources by some value.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="actionRoute">The action route.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        public static DynamicControllerComposedTemplate WriteDeleteByController<TRequest>(
            this DynamicControllerFactory factory,
            string actionRoute = null,
            string nameSpace = "DynamicControllers",
            string controllerName = null,
            string controllerRoute = "api/[controller]"
        )
            where TRequest : new()
        {
            var action = Templates.GetDynamicAction("DeleteBy", actionRoute);

            var template = new DynamicControllerComposedTemplate(nameSpace, controllerRoute, action);

            ParseAndReplace(factory,
                template,
                typeof(TRequest),
                null,
                false,
                req => controllerName.ToPascalCase() ?? $"DeleteBy{req}Controller"
            );

            return template;
        }

        /// <summary>
        ///     Creates a <see cref="DynamicControllerComposedTemplate"/> for updating a resources via PUT.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="actionRoute">The action route.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        public static DynamicControllerComposedTemplate WritePutController<TRequest>(
            this DynamicControllerFactory factory,
            string actionRoute = null,
            string nameSpace = "DynamicControllers",
            string controllerName = null,
            string controllerRoute = "api/[controller]"
        )
            where TRequest : new()
        {
            var action = Templates.GetDynamicAction("PutBy", actionRoute);

            var template = new DynamicControllerComposedTemplate(nameSpace, controllerRoute, action);

            ParseAndReplace(factory,
                template,
                typeof(TRequest),
                null,
                false,
                req => controllerName.ToPascalCase() ?? $"PutBy{req}Controller"
            );

            return template;
        }

        /// <summary>
        ///     Creates a <see cref="DynamicControllerComposedTemplate"/> for creating a resources.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="actionRoute">The action route.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>DynamicControllerComposedTemplate.</returns>
        /// <exception cref="ArgumentException">Must be a single object type. - TResponse</exception>
        public static DynamicControllerComposedTemplate WritePostController<TRequest, TResponse>(
            this DynamicControllerFactory factory,
            string actionRoute = null,
            string nameSpace = "DynamicControllers",
            string controllerName = null,
            string controllerRoute = "api/[controller]"
        )
            where TRequest : new()
        {
            if (typeof(TResponse).IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException("Must be a single object type.", nameof(TResponse));

            var action = Templates.GetDynamicAction("Post", actionRoute);

            var template = new DynamicControllerComposedTemplate(nameSpace, controllerRoute, action);

            ParseAndReplace(factory,
                template,
                typeof(TRequest),
                typeof(TResponse),
                false,
                req => controllerName.ToPascalCase() ?? $"Post{req}Controller"
            );

            return template;
        }


        private static void ParseAndReplace(
            DynamicControllerFactory factory,
            IComposedTemplate template,
            Type requestObj,
            Type responseObjectResult,
            bool requestObjMustHaveBody,
            Func<string, string> buildControllerName
        )
        {
            factory.ValidateRequestType(requestObj, requestObjMustHaveBody);

            if (!(responseObjectResult is null))
                DynamicControllerFactory.ValidateResponseType(responseObjectResult);

            var (reqObj, _, requestNs) = factory.WriteNameWithNamespaces(requestObj);
            var (parameters, paramNs) = factory.WriteInputParamsWithNamespaces(requestObj);
            var parameterComments = string.Join(Environment.NewLine, factory.WriteInputParamsXmlComments(requestObj));
            var (resObj, realResObject, resultObjNs) = responseObjectResult is null
                ? ("", "", null)
                : factory.WriteNameWithNamespaces(responseObjectResult);
            var newReqObj = factory.WriteNewObject(requestObj);

            var ns = template.Action.Namespaces
                .Union(paramNs)
                .Union(requestNs);

            if (!(resultObjNs is null))
                ns = ns.Union(resultObjNs);

            template.Action.Namespaces = ns.ToArray();

            var cn = buildControllerName(realResObject);

            template.Name = cn;

            template.Action.Tokens["[REQ_OBJECT]"] = reqObj;
            template.Action.Tokens["[RES_OBJECT_INNER_RESULT]"] = realResObject ?? resObj;
            template.Action.Tokens["[ACTION_ROUTE]"] = template.Action.Route ?? "";
            template.Action.Tokens["[PARAMS_COMMENTS]"] = parameterComments;
            template.Action.Tokens["[RES_OBJECT_RESULT]"] = resObj;
            template.Action.Tokens["[ACTION_PARAMS]"] = parameters;
            template.Action.Tokens["[NEW_REQ_OBJECT]"] = newReqObj;
            template.Action.Tokens["[CONTROLLER_NAME]"] = cn;

            template.Action.Compose();
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
                    bits["Ctor"],
                    bits["Fields"].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries),
                    bits["Namespaces"].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries),
                    methods
                );
            }
        }
    }
}
