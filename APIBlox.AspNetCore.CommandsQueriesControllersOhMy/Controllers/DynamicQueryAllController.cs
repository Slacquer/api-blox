using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Controllers
{
    /// <summary>
    ///     Class DynamicQueryAllController.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <typeparam name="TResponse">The type of the t response.</typeparam>
    /// <typeparam name="TId">The type of the t id in the response</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest, TResponse, TId}" />
    [Route("api/[controller]")]
    [ApiController]
    public sealed class DynamicQueryAllController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TResponse : IResource<TId>
    {
        private readonly IQueryHandler<TRequest, HandlerResponse> _getAllHandler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicQueryAllController{TRequest, TResponse, TId}" /> class.
        /// </summary>
        /// <param name="getAllHandler">The get all handler.</param>
        public DynamicQueryAllController(IQueryHandler<TRequest, HandlerResponse> getAllHandler)
        {
            _getAllHandler = getAllHandler;
        }

        /// <summary>
        ///     Action for getting a collection of resources.
        ///     <para>
        ///         Responses: 200, 204, 401, 403
        ///     </para>
        /// </summary>
        /// <param name="request">TRequest input parameter(s)</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromRoute] TRequest request, CancellationToken cancellationToken)
        {
            var ret = await _getAllHandler.HandleAsync(request, cancellationToken).ConfigureAwait(false);

            if (ret.HasErrors)
                return new ProblemResult(ret.Error);

            return ret.Result is null ? NoContent() : (IActionResult) Ok(ret.Result);
        }
    }

        /// <summary>
    ///     Class DynamicQueryAllController.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <typeparam name="TResponse">The type of the t response.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest, TResponse}" />
    [Route("api/[controller]")]
    [ApiController]
    public sealed class DynamicQueryAllController<TRequest, TResponse> : ControllerBase,
        IDynamicController<TRequest, TResponse>
        where TResponse : IResource
    {
        private readonly IQueryHandler<TRequest, HandlerResponse> _getAllHandler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicQueryAllController{TRequest, TResponse}" /> class.
        /// </summary>
        /// <param name="getAllHandler">The get all handler.</param>
        public DynamicQueryAllController(IQueryHandler<TRequest, HandlerResponse> getAllHandler)
        {
            _getAllHandler = getAllHandler;
        }

        /// <summary>
        ///     Action for getting a collection of resources.
        ///     <para>
        ///         Responses: 200, 204, 401, 403
        ///     </para>
        /// </summary>
        /// <param name="request">TRequest input parameter(s)</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromRoute] TRequest request, CancellationToken cancellationToken)
        {
            var ret = await _getAllHandler.HandleAsync(request, cancellationToken).ConfigureAwait(false);

            if (ret.HasErrors)
                return new ProblemResult(ret.Error);

            return ret.Result is null ? NoContent() : (IActionResult) Ok(ret.Result);
        }
    }
}
