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


        public static string ComposeGetAllController<TRequest, TResponse>(
            this DynamicControllerFactory factory, string controllerName = null
        )
        {
            var contents = Contents(typeof(TRequest),
                typeof(TResponse),
                Templates.GetAllTemplate,
                (req, res) => controllerName ?? $"GetAll{res}Using{req}Controller"
            );

            return contents;
        }


        private static string Contents(Type requestObj, Type responseObjectResult, string template, Func<string, string, string> buildControllerName)
        {
            var (reqObj, _, requestNs) = DynamicControllerFactory.Helpers.ToStringWithNamespaces(requestObj);
            var (parameters, paramNs) = DynamicControllerFactory.Helpers.ToInputParams(requestObj);
            var (resObj, realResObject, resultObjNs) = DynamicControllerFactory.Helpers.ToStringWithNamespaces(responseObjectResult);

            var ns = string.Join(Environment.NewLine,
                DefaultNamespaces
                    .Union(paramNs)
                    .Union(requestNs)
                    .Union(resultObjNs)
                    .OrderBy(s => s).Select(s => $"using {s};")
            );

            var cn = buildControllerName(reqObj, realResObject??resObj);

            var contents = template.Replace("@ControllerName", cn)
                .Replace("@namespaces", ns)
                .Replace("@RequestObj", reqObj)
                .Replace("@ResponseObjResult", resObj)
                .Replace("@RealResponseObjResult", realResObject??resObj)
                .Replace("@ResponseObj", typeof(HandlerResponse).Name)
                .Replace("@params", parameters);
            return contents;
        }

        private static class Templates
        {
            public const string GetAllTemplate = @"
@namespaces

namespace SomeNameSpace.Controllers
{
    [Route(""api/[controller]"")]
    [ApiController]
    public sealed class @ControllerName : ControllerBase
    {
        private readonly IQueryHandler<@RequestObj, @ResponseObj> _getAllHandler;

        public @ControllerName(IQueryHandler<@RequestObj, @ResponseObj> getAllHandler)
        {
            _getAllHandler = getAllHandler;
        }

        /// <summary>
        ///     Action for getting a collection of resources.
        ///     <para>
        ///         Responses: 200, 204, 401, 403
        ///     </para>
        /// </summary>
        /// <param name=""request"">@RequestObj input parameter(s)</param>
        /// <param name=""cancellationToken"">The cancellation token.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(@ResponseObjResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll@RealResponseObjResult(@params, CancellationToken cancellationToken)
        {
            var obj = new @RequestObj();

            var ret = await _getAllHandler.HandleAsync(obj, cancellationToken).ConfigureAwait(false);

            if (ret.HasErrors)
                return new ProblemResult(ret.Error);

            return ret.Result is null ? NoContent() : (IActionResult) Ok(ret.Result);
        }
    }
}
";
        }
    }
}
