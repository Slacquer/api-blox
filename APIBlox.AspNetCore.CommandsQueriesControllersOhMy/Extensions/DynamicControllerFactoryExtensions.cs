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
        ///     Creates a DynamicGetAllController
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="productionBuild">if set to <c>true</c> [production build].</param>
        /// <returns>System.ValueTuple&lt;Type, IEnumerable&lt;System.String&gt;&gt;.</returns>
        public static (Type, IEnumerable<string>) MakeGetAllController<TRequest, TResponse>(
            this DynamicControllerFactory factory, string controllerName = null,
            string assemblyName = "DynamicControllers", bool productionBuild = false
        )
        {
            var contents = Contents(typeof(TRequest),
                typeof(TResponse),
                Templates.GetAllTemplate,
                (req, res) => controllerName ?? $"GetAll{res}Using{req}Controller"
            );

            var (types, errors) = DynamicControllerFactory.Make(contents, assemblyName, !productionBuild);

            return (types?.FirstOrDefault(), errors);
        }


        private static string Contents(Type requestObj, Type responseObjectResult, string template, Func<string, string, string> buildControllerName)
        {
            var (reqObj, requestNs) = DynamicControllerFactory.Helpers.ToStringWithNamespaces(requestObj);
            var (parameters, paramNs) = DynamicControllerFactory.Helpers.ToInputParams(requestObj);
            var (resObj, resultObjNs) = DynamicControllerFactory.Helpers.ToStringWithNamespaces(responseObjectResult);

            var ns = string.Join(Environment.NewLine,
                DefaultNamespaces
                    .Union(paramNs)
                    .Union(requestNs)
                    .Union(resultObjNs)
                    .OrderBy(s => s).Select(s => $"using {s};")
            );

            var cn = buildControllerName(reqObj, resObj);

            var contents = template.Replace("@ControllerName", cn)
                .Replace("@namespaces", ns)
                .Replace("@RequestObj", reqObj)
                .Replace("@ResponseObjResult", resObj)
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
        public async Task<IActionResult> GetAll@ResponseObjResult(@params, CancellationToken cancellationToken)
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
