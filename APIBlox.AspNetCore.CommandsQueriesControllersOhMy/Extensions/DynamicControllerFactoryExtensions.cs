using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Types;

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
            "System.Collections.Generic",
            "System.Threading",
            "System.Threading.Tasks",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Mvc",
            "APIBlox.AspNetCore.ActionResults",
            "APIBlox.AspNetCore.Contracts",
            "APIBlox.AspNetCore.Types"
        };

        
        public static string ComposeQueryByController<TRequest, TResponse>(
            this DynamicControllerFactory factory, string controllerName = null,
            string controllerRoute = "api/[controller]", string actionRoute = null
        )
            where TRequest : new()
        {
            var getTemplate = Templates.GetTemplate("DynamicQueryByController");
            var contents = Contents(typeof(TRequest),
                typeof(TResponse),
                controllerRoute,
                actionRoute,
                getTemplate,
                (req, res) => controllerName ?? $"GetAll{res}Using{req}Controller"
            );

            return contents;
        }

        public static string ComposeQueryAllController<TRequest, TResponse>(
            this DynamicControllerFactory factory, string controllerName = null,
            string controllerRoute = "api/[controller]", string actionRoute = null
        )
            where TRequest : new()
        {
            var getTemplate = Templates.GetTemplate("DynamicQueryAllController");
            var contents = Contents(typeof(TRequest),
                typeof(TResponse),
                controllerRoute,
                actionRoute,
                getTemplate,
                (req, res) => controllerName ?? $"GetAll{res}Using{req}Controller"
            );

            return contents;
        }

        private static string Contents(Type requestObj, Type responseObjectResult,
            string controllerRoute, string actionRoute, string template,
            Func<string, string, string> buildControllerName
        )
        {
            var (reqObj, _, requestNs) = DynamicControllerFactory.Helpers.GetNameWithNamespaces(requestObj);
            var (parameters, paramNs) = DynamicControllerFactory.Helpers.GetInputParamsWithNamespaces(requestObj);
            var (resObj, realResObject, resultObjNs) = DynamicControllerFactory.Helpers.GetNameWithNamespaces(responseObjectResult);
            var newReqObj = DynamicControllerFactory.Helpers.GetNewObject(requestObj);

            var ns = string.Join(Environment.NewLine,
                DefaultNamespaces
                    .Union(paramNs)
                    .Union(requestNs)
                    .Union(resultObjNs)
                    .OrderBy(s => s).Select(s => $"using {s};")
            );

            var cn = buildControllerName(reqObj, realResObject ?? resObj);

            var contents = template.Replace("[CONTROLLER_NAME]", cn)
                .Replace("[NAMESPACES]", ns)
                .Replace("[CONTROLLER_ROUTE]", controllerRoute)
                .Replace("[ACTION_ROUTE]", actionRoute is null ? "" : $"({actionRoute})")
                .Replace("[REQ_OBJECT]", reqObj)
                .Replace("[REQ_OBJECT_RESULT]", resObj)
                .Replace("[REAL_RES_OBJECT_RESULT", realResObject ?? resObj)
                .Replace("[RES_OBJECT]", typeof(HandlerResponse).Name)
                .Replace("[NEW_REQ_OBJECT]", newReqObj)
                .Replace("[ACTION_PARAMS]", parameters);
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
