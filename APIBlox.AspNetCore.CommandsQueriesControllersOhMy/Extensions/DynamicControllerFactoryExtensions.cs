using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
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
            "System.Collections.Generic",
            "System.Threading",
            "System.Threading.Tasks",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Mvc",
            "APIBlox.AspNetCore.ActionResults",
            "APIBlox.AspNetCore.Contracts",
            "APIBlox.AspNetCore.Types",
            "APIBlox.AspNetCore.Types.Errors"
        };


        public static IComposedTemplate ComposeQueryByController<TRequest, TResponse>(
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
                (req, res) => controllerName ?? $"QueryBy{res}Controller"
            );

            return new DynamicControllerComposedTemplate(contents);
        }

        public static IComposedTemplate ComposeQueryAllController<TRequest, TResponse>(
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
                (req, res) => controllerName ?? $"QueryAll{res}Controller"
            );

            return new DynamicControllerComposedTemplate(contents);
        }

        private static string Contents(Type requestObj, Type responseObjectResult,
            string controllerRoute, string actionRoute, string template,
            Func<string, string, string> buildControllerName
        )
        {
            var (reqObj, _, requestNs) = DynamicControllerFactory.Helpers.GetNameWithNamespaces(requestObj);
            var (parameters, paramNs) = DynamicControllerFactory.Helpers.GetInputParamsWithNamespaces(requestObj);
            var parameterComments = string.Join(Environment.NewLine, DynamicControllerFactory.Helpers.GetInputParamsXmlComments(requestObj));
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

            // XML Comments need to be read for input params, summary, remarks etc.
            // https://stackoverflow.com/questions/15602606/programmatically-get-summary-comments-at-runtime

            var contents = template.Replace("[CONTROLLER_NAME]", cn)
                .Replace("[NAMESPACES]", ns)
                .Replace("[CONTROLLER_ROUTE]", controllerRoute)
                .Replace("[ACTION_ROUTE]", actionRoute is null ? "" : $"({actionRoute})")
                .Replace("[REQ_OBJECT]", reqObj)
                .Replace("[RES_OBJECT_RESULT]", resObj)
                .Replace("[RES_OBJECT_INNER_RESULT]", realResObject ?? resObj)
                .Replace("[NEW_REQ_OBJECT]", newReqObj)
                .Replace("[ACTION_PARAMS]", parameters)
                .Replace("[PARAMS_COMMENTS]", parameterComments);
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