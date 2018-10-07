#region -    Using Statements    -

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.CommandQueryResponses;
using APIBlox.AspNetCore.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace APIBlox.AspNetCore.Controllers
{
    /// <summary>
    ///     Class DynamicQueryAllController.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <typeparam name="TResponse">The type of the t response.</typeparam>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest, TResponse, TId}" />
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicQueryAllController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TResponse : IResource<TId>
    {
        #region -    Fields    -

        private readonly IQueryHandler<TRequest, HandlerResponse> _getAllHandler;

        private readonly string _rn = typeof(TRequest).Name;

        #endregion

        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicQueryAllController{TRequest, TResponse, TId}" /> class.
        /// </summary>
        /// <param name="getAllHandler">The get all handler.</param>
        public DynamicQueryAllController(IQueryHandler<TRequest, HandlerResponse> getAllHandler)
        {
            _getAllHandler = getAllHandler;
        }

        #endregion

        /// <summary>
        ///     Action for getting a collection of resources.
        ///     <para>
        ///         Responses: 200, 204, 401, 403
        ///     </para>
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public virtual async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var req = (TRequest) RouteData.Values[_rn];
            var ret = await _getAllHandler.HandleAsync(req, cancellationToken).ConfigureAwait(false);

            return ret.HasErrors
                ? new ProblemResult(ret.Error)
                : Ok(ret.Result);
        }
    }
}
