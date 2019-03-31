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
            "System.Threading",
            "System.Threading.Tasks",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Mvc",
            "APIBlox.AspNetCore.ActionResults",
            "APIBlox.AspNetCore.Contracts",
            "APIBlox.AspNetCore.Types"
        };


        public static IComposedTemplate WriteQueryByController<TRequest, TResponse>(
            this DynamicControllerFactory factory, string name = null,
            string nameSpace = "DynamicControllers",
            string route = "api/[controller]", string actionRoute = null
        )
            where TRequest : new()
        {
            var getTemplate = Templates.GetTemplate("DynamicQueryByController");
            var contents = Contents(
                DefaultNamespaces,
                nameSpace,
                typeof(TRequest),
                typeof(TResponse),
                route,
                actionRoute,
                getTemplate,
                (req, res) => name ?? $"QueryBy{res}Controller"
            );

            return new DynamicControllerComposedTemplate(contents);
        }

        public static IComposedTemplate WriteQueryAllController<TRequest, TResponse>(
            this DynamicControllerFactory factory, string name = null,
            string nameSpace = "DynamicControllers",
            string route = "api/[controller]", string actionRoute = null
        )
            where TRequest : new()
        {
            var ns = new List<string>();
            ns.AddRange(DefaultNamespaces);
            ns.Add("System.Collections.Generic");

            var getTemplate = Templates.GetTemplate("DynamicQueryAllController");
            var contents = Contents(
               ns,
               nameSpace,
                typeof(TRequest),
                typeof(TResponse),
                route,
                actionRoute,
                getTemplate,
                (req, res) => name ?? $"QueryAll{res}Controller"
            );

            return new DynamicControllerComposedTemplate(contents);
        }

        private static string Contents(IEnumerable<string> namespaces, string controllersNamespace, Type requestObj, Type responseObjectResult,
            string controllerRoute, string actionRoute, string template,
            Func<string, string, string> buildControllerName
        )
        {
            DynamicControllerFactory.Helpers.ValidateRequestType(requestObj);
            DynamicControllerFactory.Helpers.ValidateResponseType(responseObjectResult);

            var (reqObj, _, requestNs) = DynamicControllerFactory.Helpers.WriteNameWithNamespaces(requestObj);
            var (parameters, paramNs) = DynamicControllerFactory.Helpers.WriteInputParamsWithNamespaces(requestObj);
            var parameterComments = string.Join(Environment.NewLine, DynamicControllerFactory.Helpers.WriteInputParamsXmlComments(requestObj));
            var (resObj, realResObject, resultObjNs) = DynamicControllerFactory.Helpers.WriteNameWithNamespaces(responseObjectResult);
            var newReqObj = DynamicControllerFactory.Helpers.WriteNewObject(requestObj);

            var ns = string.Join(Environment.NewLine,
                namespaces
                    .Union(paramNs)
                    .Union(requestNs)
                    .Union(resultObjNs)
                    .OrderBy(s => s).Select(s => $"using {s};")
            );

            var cn = buildControllerName(reqObj, realResObject ?? resObj);

            // XML Comments need to be read for input params, summary, remarks etc.
            // https://stackoverflow.com/questions/15602606/programmatically-get-summary-comments-at-runtime

            var contents = template
                .Replace("[CONTROLLERS_NAMESPACE]", controllersNamespace)
                .Replace("[CONTROLLER_NAME]", cn)
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