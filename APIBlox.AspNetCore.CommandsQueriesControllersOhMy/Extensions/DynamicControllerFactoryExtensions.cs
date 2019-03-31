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
        private static readonly List<string> DefaultNamespaces = new List<string>
        {
            "System",
            "System.Threading",
            "System.Threading.Tasks",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Mvc",
            "APIBlox.AspNetCore.ActionResults",
            "APIBlox.AspNetCore.Contracts",
            "APIBlox.AspNetCore.Types"
        };


        public static DynamicControllerComposedTemplate WriteQueryByController<TRequest, TResponse>(
            this DynamicControllerFactory factory, string controllerName = null,
            string nameSpace = "DynamicControllers",
            string controllerRoute = "api/[controller]", string actionRoute = null
        )
            where TRequest : new()
        {
            if (typeof(TResponse).IsAssignableTo(typeof(IEnumerable)))
                throw new ArgumentException("Must be a single object type.", nameof(TResponse));

            var getTemplate = Templates.GetTemplate("DynamicQueryByController");
            var contents = ContentsWithResults(
                DefaultNamespaces,
                nameSpace,
                typeof(TRequest),
                typeof(TResponse),
                false,
                controllerRoute,
                actionRoute,
                getTemplate,
                (req, res) => controllerName.ToPascalCase() ?? $"QueryBy{res}Controller"
            );

            var queryHandler = $"IQueryHandler<{typeof(TRequest).Name}, HandlerResponse>";

            return new DynamicControllerComposedTemplate(contents, queryHandler);
        }

        public static DynamicControllerComposedTemplate WriteQueryAllController<TRequest, TResponse>(
            this DynamicControllerFactory factory, string controllerName = null,
            string nameSpace = "DynamicControllers",
            string controllerRoute = "api/[controller]", string actionRoute = null
        )
            where TRequest : new()
            where TResponse : IEnumerable
        {
            var ns = new List<string>();
            ns.AddRange(DefaultNamespaces);
            ns.Add("System.Collections.Generic");

            var getTemplate = Templates.GetTemplate("DynamicQueryAllController");
            var contents = ContentsWithResults(
                ns,
                nameSpace,
                typeof(TRequest),
                typeof(TResponse),
                false,
                controllerRoute,
                actionRoute,
                getTemplate,
                (req, res) => controllerName.ToPascalCase() ?? $"QueryAll{res}Controller"
            );

            var queryHandler = $"IQueryHandler<{typeof(TRequest).Name}, HandlerResponse>";

            return new DynamicControllerComposedTemplate(contents, queryHandler);
        }

        public static DynamicControllerComposedTemplate WriteDeleteByController<TRequest>(
            this DynamicControllerFactory factory, string controllerName = null,
            string nameSpace = "DynamicControllers",
            string controllerRoute = "api/[controller]", string actionRoute = null
        )
            where TRequest : new()
        {
            var getTemplate = Templates.GetTemplate("DynamicDeleteByController");
            var contents = Contents(
                DefaultNamespaces,
                nameSpace,
                typeof(TRequest),
                false,
                controllerRoute,
                actionRoute,
                getTemplate,
                (req) => controllerName.ToPascalCase() ?? $"DeleteUsing{req}Controller"
            );

            var cmdHandler = $"ICommandHandler<{typeof(TRequest).Name}, HandlerResponse>";

            return new DynamicControllerComposedTemplate(contents, cmdHandler);
        }

        public static DynamicControllerComposedTemplate WritePutController<TRequest>(
            this DynamicControllerFactory factory, string controllerName = null,
            string nameSpace = "DynamicControllers",
            string controllerRoute = "api/[controller]", string actionRoute = null
        )
            where TRequest : new()
        {
            var getTemplate = Templates.GetTemplate("DynamicPutController");
            var contents = Contents(
                DefaultNamespaces,
                nameSpace,
                typeof(TRequest),
                true,
                controllerRoute,
                actionRoute,
                getTemplate,
                (req) => controllerName.ToPascalCase() ?? $"PutUsing{req}Controller"
            );

            var cmdHandler = $"ICommandHandler<{typeof(TRequest).Name}, HandlerResponse>";

            return new DynamicControllerComposedTemplate(contents, cmdHandler);
        }

        public static DynamicControllerComposedTemplate WritePostController<TRequest, TResponse>(
            this DynamicControllerFactory factory, string controllerName = null,
            string nameSpace = "DynamicControllers",
            string controllerRoute = "api/[controller]", string actionRoute = null
        )
            where TRequest : new()
        {
            var ns = new List<string>();
            ns.AddRange(DefaultNamespaces);
            ns.Add("System.Linq");
            ns.Add("Microsoft.Extensions.Logging");
            ns.Add("APIBlox.NetCore.Extensions");

            var getTemplate = Templates.GetTemplate("DynamicPostController");
            var contents = ContentsWithResults(
                ns,
                nameSpace,
                typeof(TRequest),
                typeof(TResponse),
                true,
                controllerRoute,
                actionRoute,
                getTemplate,
                (req, res) => controllerName.ToPascalCase() ?? $"PostUsing{req}Controller"
            );

            var cmdHandler = $"ICommandHandler<{typeof(TRequest).Name}, HandlerResponse>";

            return new DynamicControllerComposedTemplate(contents, cmdHandler);
        }
        

        private static string Contents(IEnumerable<string> namespaces, string controllersNamespace, Type requestObj,
            bool requestObjMustHaveBody, string controllerRoute, string actionRoute, string template,
           Func<string, string> buildControllerName
       )
        {
            DynamicControllerFactory.Helpers.ValidateRequestType(requestObj, requestObjMustHaveBody);

            var (reqObj, _, requestNs) = DynamicControllerFactory.Helpers.WriteNameWithNamespaces(requestObj);
            var (parameters, paramNs) = DynamicControllerFactory.Helpers.WriteInputParamsWithNamespaces(requestObj);
            var parameterComments = string.Join(Environment.NewLine, DynamicControllerFactory.Helpers.WriteInputParamsXmlComments(requestObj));

            var newReqObj = DynamicControllerFactory.Helpers.WriteNewObject(requestObj);

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

        private static string ContentsWithResults(IEnumerable<string> namespaces, string controllersNamespace, Type requestObj, Type responseObjectResult,
            bool requestObjMustHaveBody,
            string controllerRoute, string actionRoute, string template,
            Func<string, string, string> buildControllerName
        )
        {
            DynamicControllerFactory.Helpers.ValidateRequestType(requestObj, requestObjMustHaveBody);

            if (!(responseObjectResult is null))
                DynamicControllerFactory.Helpers.ValidateResponseType(responseObjectResult);

            var (reqObj, _, requestNs) = DynamicControllerFactory.Helpers.WriteNameWithNamespaces(requestObj);
            var (parameters, paramNs) = DynamicControllerFactory.Helpers.WriteInputParamsWithNamespaces(requestObj);
            var parameterComments = string.Join(Environment.NewLine, DynamicControllerFactory.Helpers.WriteInputParamsXmlComments(requestObj));
            var (resObj, realResObject, resultObjNs) = responseObjectResult is null
                ? (null, null, null)
                : DynamicControllerFactory.Helpers.WriteNameWithNamespaces(responseObjectResult);
            var newReqObj = DynamicControllerFactory.Helpers.WriteNewObject(requestObj);

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
            public static string GetTemplate(string template)
            {
                return EmbeddedResourceReader<Templates>.GetResource($"{template}.txt");
            }
        }
    }


}