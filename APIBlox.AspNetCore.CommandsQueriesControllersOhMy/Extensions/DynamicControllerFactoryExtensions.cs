using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Types;

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

        /// <summary>
        /// Composes the get all controller.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <returns>System.String.</returns>
        public static string ComposeGetAllController<TRequest, TResponse>(
            this DynamicControllerFactory factory, string controllerName = null,
            string controllerRoute = "api/[controller]", string actionRoute = null
        ) where TRequest : new()
        {
            var contents = Contents(typeof(TRequest),
                typeof(TResponse),
                controllerRoute,
                actionRoute,
                Templates.GetAllTemplate,
                (req, res) => controllerName ?? $"GetAll{res}Using{req}Controller"
            );

            return contents;
        }


        private static string Contents(Type requestObj, Type responseObjectResult,
            string controllerRoute, string actionRoute, string template,
            Func<string, string, string> buildControllerName)
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

        private static class Templates
        {
            public const string GetAllTemplate = @"
[NAMESPACES]

namespace SomeNameSpace.Controllers
{
    [Route(""[CONTROLLER_ROUTE]"")]
    [ApiController]
    public sealed class [CONTROLLER_NAME] : ControllerBase
    {
        private readonly IQueryHandler<[REQ_OBJECT], [RES_OBJECT]> _getAllHandler;

        public [CONTROLLER_NAME](IQueryHandler<[REQ_OBJECT], [RES_OBJECT]> getAllHandler)
        {
            _getAllHandler = getAllHandler;
        }

        [HttpGet[ACTION_ROUTE]]
        [ProducesResponseType(typeof([REQ_OBJECT_RESULT]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll[REAL_RES_OBJECT_RESULT(
            [ACTION_PARAMS], 
            CancellationToken cancellationToken
        )
        {
            var ret = await _getAllHandler.HandleAsync(
                [NEW_REQ_OBJECT], 
                cancellationToken
            ).ConfigureAwait(false);

            if (ret.HasErrors)
                return new ProblemResult(ret.Error);

            return ret.Result is null 
                ? NoContent() 
                : (IActionResult) Ok(ret.Result);
        }
    }
}
";
        }
    }
}
