using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Types;
using APIBlox.NetCore.Extensions;

namespace APIBlox.AspNetCore.Extensions
{
    /// <summary>
    ///     Class DynamicControllerFactoryExtensions.
    /// </summary>
    public static class DynamicControllerFactoryExtensions
    {
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
                throw new ArgumentException($"QueryBy requires a route for the action, maybe something like {{id}}.",
                    nameof(actionRoute)
                );

            if (typeof(TResponse).IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException("Must be a single object type.", nameof(TResponse));

            var action = Templates.GetDynamicAction("QueryBy");
            action.Name = "QueryBy";
            action.Route = actionRoute;

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


        public static DynamicControllerComposedTemplate WriteQueryAllController<TRequest, TResponse>(
            this DynamicControllerFactory factory,
            string nameSpace = "DynamicControllers",
            string controllerName = null,
            string controllerRoute = "api/[controller]",
            string actionRoute = null
        )
            where TRequest : new()
            where TResponse : IEnumerable
        {
            if (!typeof(TResponse).IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException("Must be a enumerable object type.", nameof(TResponse));

            var action = Templates.GetDynamicAction("QueryAll");
            action.Name = "QueryAll";
            action.Route = actionRoute;

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


        private static void ParseAndReplace(
            DynamicControllerFactory factory,
            DynamicControllerComposedTemplate template,
            Type requestObj,
            Type responseObjectResult,
            bool requestObjMustHaveBody,
            Func<string, string> buildControllerName
)
        {
            factory.ValidateRequestType(requestObj, requestObjMustHaveBody);

            if (!(responseObjectResult is null))
                factory.ValidateResponseType(responseObjectResult);

            var (reqObj, _, requestNs) = factory.WriteNameWithNamespaces(requestObj);
            var (parameters, paramNs) = factory.WriteInputParamsWithNamespaces(requestObj);
            var parameterComments = string.Join(Environment.NewLine, factory.WriteInputParamsXmlComments(requestObj));
            var (resObj, realResObject, resultObjNs) = responseObjectResult is null
                ? (null, null, null)
                : factory.WriteNameWithNamespaces(responseObjectResult);
            var newReqObj = factory.WriteNewObject(requestObj);

            template.Action.Namespaces = template.Action.Namespaces
                .Union(paramNs)
                .Union(requestNs)
                .Union(resultObjNs)
                .OrderBy(s => s).ToArray();

            var cn = buildControllerName(realResObject);

            template.Name = cn;

            template.Action.Content = template.Action.Content
                .Replace("[REQ_OBJECT]", reqObj)
                .Replace("[RES_OBJECT_INNER_RESULT]", realResObject ?? resObj)
                .Replace("[ACTION_ROUTE]", template.Action.Route ?? "")
                .Replace("[PARAMS_COMMENTS]", parameterComments)
                .Replace("[RES_OBJECT_RESULT]", resObj)
                .Replace("[ACTION_PARAMS]", parameters)
                .Replace("[NEW_REQ_OBJECT]", newReqObj)
                .Replace("()]", "]")
                .Replace("(\"\")", "");

            template.Action.Ctor = template.Action.Ctor
                .Replace("[REQ_OBJECT]", reqObj)
                .Replace("[RES_OBJECT_INNER_RESULT]", realResObject ?? resObj)
                .Replace("[CONTROLLER_NAME]", cn);

            template.Action.Fields = template.Action.Fields.Select(s =>
                s.Replace("[REQ_OBJECT]", reqObj)
            ).ToArray();
        }

        ////public static DynamicControllerComposedTemplate WriteDeleteByController<TRequest>(
        ////    this DynamicControllerFactory factory, string controllerName = null,
        ////    string nameSpace = "DynamicControllers",
        ////    string controllerRoute = "api/[controller]", string actionRoute = null
        ////)
        ////    where TRequest : new()
        ////{
        ////    var getTemplate = Templates.GetTemplate("DynamicDeleteByController");
        ////    var contents = Contents(
        ////        factory,
        ////        DefaultNamespaces,
        ////        nameSpace,
        ////        typeof(TRequest),
        ////        false,
        ////        controllerRoute,
        ////        actionRoute,
        ////        getTemplate,
        ////        (req) => controllerName.ToPascalCase() ?? $"DeleteUsing{req}Controller"
        ////    );

        ////    var cmdHandler = $"ICommandHandler<{typeof(TRequest).Name}, HandlerResponse>";

        ////    return new DynamicControllerComposedTemplate(contents, cmdHandler);
        ////}

        ////public static DynamicControllerComposedTemplate WritePutController<TRequest>(
        ////    this DynamicControllerFactory factory, string controllerName = null,
        ////    string nameSpace = "DynamicControllers",
        ////    string controllerRoute = "api/[controller]", string actionRoute = null
        ////)
        ////    where TRequest : new()
        ////{
        ////    var getTemplate = Templates.GetTemplate("DynamicPutController");
        ////    var contents = Contents(
        ////        factory,
        ////        DefaultNamespaces,
        ////        nameSpace,
        ////        typeof(TRequest),
        ////        true,
        ////        controllerRoute,
        ////        actionRoute,
        ////        getTemplate,
        ////        (req) => controllerName.ToPascalCase() ?? $"PutUsing{req}Controller"
        ////    );

        ////    var cmdHandler = $"ICommandHandler<{typeof(TRequest).Name}, HandlerResponse>";

        ////    return new DynamicControllerComposedTemplate(contents, cmdHandler);
        ////}

        ////public static DynamicControllerComposedTemplate WritePostController<TRequest, TResponse>(
        ////    this DynamicControllerFactory factory, string controllerName = null,
        ////    string nameSpace = "DynamicControllers",
        ////    string controllerRoute = "api/[controller]", string actionRoute = null
        ////)
        ////    where TRequest : new()
        ////{
        ////    var ns = new List<string>();
        ////    ns.AddRange(DefaultNamespaces);
        ////    ns.Add("System.Linq");
        ////    ns.Add("Microsoft.Extensions.Logging");
        ////    ns.Add("APIBlox.NetCore.Extensions");

        ////    var getTemplate = Templates.GetTemplate("DynamicPostController");
        ////    var contents = ContentsWithResults(
        ////        factory,
        ////        ns,
        ////        nameSpace,
        ////        typeof(TRequest),
        ////        typeof(TResponse),
        ////        true,
        ////        controllerRoute,
        ////        actionRoute,
        ////        getTemplate,
        ////        (req, res) => controllerName.ToPascalCase() ?? $"PostUsing{req}Controller"
        ////    );

        ////    var cmdHandler = $"ICommandHandler<{typeof(TRequest).Name}, HandlerResponse>";

        ////    return new DynamicControllerComposedTemplate(contents, cmdHandler);
        ////}


        private static string Contents(
            DynamicControllerFactory factory,
            IEnumerable<string> namespaces, string controllersNamespace, Type requestObj,
            bool requestObjMustHaveBody, string controllerRoute, string actionRoute, string template,
           Func<string, string> buildControllerName
       )
        {
            factory.ValidateRequestType(requestObj, requestObjMustHaveBody);

            var (reqObj, _, requestNs) = factory.WriteNameWithNamespaces(requestObj);
            var (parameters, paramNs) = factory.WriteInputParamsWithNamespaces(requestObj);
            var parameterComments = string.Join(Environment.NewLine, factory.WriteInputParamsXmlComments(requestObj));

            var newReqObj = factory.WriteNewObject(requestObj);

            var ns = string.Join(Environment.NewLine,
                namespaces
                    .Union(paramNs)
                    .Union(requestNs)
                    .OrderBy(s => s).Select(s => $"using {s};")
            );

            var cn = buildControllerName(reqObj);

            var contents = template
                .Replace("[CONTROLLERS_NAMESPACE]", controllersNamespace)
                .Replace("[CONTROLLER_NAME]", cn)
                .Replace("[NAMESPACES]", ns)
                .Replace("[CONTROLLER_ROUTE]", controllerRoute)
                .Replace("[ACTION_ROUTE]", actionRoute ?? "")
                .Replace("[REQ_OBJECT]", reqObj)
                .Replace("[NEW_REQ_OBJECT]", newReqObj)
                .Replace("[ACTION_PARAMS]", parameters)
                .Replace("[PARAMS_COMMENTS]", parameterComments)
                // lame but its bugging me.
                .Replace("()]", "]")
                .Replace("(\"\")", "");
            return contents;
        }

        private static string ContentsWithResults(DynamicControllerFactory factory, IEnumerable<string> namespaces,
            string controllersNamespace, Type requestObj, Type responseObjectResult,
            bool requestObjMustHaveBody,
            string controllerRoute, string actionRoute, string template,
            Func<string, string, string> buildControllerName
        )
        {
            factory.ValidateRequestType(requestObj, requestObjMustHaveBody);

            if (!(responseObjectResult is null))
                factory.ValidateResponseType(responseObjectResult);

            var (reqObj, _, requestNs) = factory.WriteNameWithNamespaces(requestObj);
            var (parameters, paramNs) = factory.WriteInputParamsWithNamespaces(requestObj);
            var parameterComments = string.Join(Environment.NewLine, factory.WriteInputParamsXmlComments(requestObj));
            var (resObj, realResObject, resultObjNs) = responseObjectResult is null
                ? (null, null, null)
                : factory.WriteNameWithNamespaces(responseObjectResult);
            var newReqObj = factory.WriteNewObject(requestObj);

            var ns = string.Join(Environment.NewLine,
                namespaces
                    .Union(paramNs)
                    .Union(requestNs)
                    .Union(resultObjNs)
                    .OrderBy(s => s).Select(s => $"using {s};")
            );

            var cn = buildControllerName(reqObj, realResObject ?? resObj);

            var contents = template
                .Replace("[CONTROLLERS_NAMESPACE]", controllersNamespace)
                .Replace("[CONTROLLER_NAME]", cn)
                .Replace("[NAMESPACES]", ns)
                .Replace("[CONTROLLER_ROUTE]", controllerRoute)
                .Replace("[ACTION_ROUTE]", actionRoute ?? "")
                .Replace("[REQ_OBJECT]", reqObj)
                .Replace("[RES_OBJECT_RESULT]", resObj)
                .Replace("[RES_OBJECT_INNER_RESULT]", realResObject ?? resObj)
                .Replace("[NEW_REQ_OBJECT]", newReqObj)
                .Replace("[ACTION_PARAMS]", parameters)
                .Replace("[PARAMS_COMMENTS]", parameterComments)
                // lame but its bugging me.
                .Replace("()]", "]")
                .Replace("(\"\")", "");
            return contents;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Templates
        {
            public static DynamicAction GetDynamicAction(string templatePath)
            {
                var bits = EmbeddedResourceReader<Templates>.GetResources(templatePath);

                return new DynamicAction
                {
                    Content = bits["ActionContent"],
                    Ctor = bits["Ctor"],
                    Fields = bits["Fields"].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries),
                    Namespaces = bits["Namespaces"].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries),
                };
            }
        }

    }


}