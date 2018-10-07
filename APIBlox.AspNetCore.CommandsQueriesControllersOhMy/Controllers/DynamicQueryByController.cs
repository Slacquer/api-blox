﻿#region -    Using Statements    -

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
    ///     Class DynamicQueryByController.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <typeparam name="TResponse">The type of the t response.</typeparam>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest, TResponse, TId}" />
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicQueryByController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TResponse : IResource<TId>
    {
        #region -    Fields    -

        private readonly IQueryHandler<TRequest, HandlerResponse> _getByHandler;
        private readonly string _rn = typeof(TRequest).Name;

        #endregion

        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicQueryByController{TRequest, TResponse, TId}" /> class.
        /// </summary>
        /// <param name="getByHandler">The get by handler.</param>
        public DynamicQueryByController(IQueryHandler<TRequest, HandlerResponse> getByHandler)
        {
            _getByHandler = getByHandler;
        }

        #endregion

        /// <summary>
        ///     Action for getting a resource.
        ///     <para>
        ///         Responses: 200, 401, 403, 404
        ///     </para>
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var req = (TRequest) RouteData.Values[_rn];
            var ret = await _getByHandler.HandleAsync(req, cancellationToken).ConfigureAwait(false);

            return ret.HasErrors
                ? new ProblemResult(ret.Error)
                : Ok(ret.Result);
        }
    }
}
