using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

            return ret.Result is null ? NoContent() : (IActionResult)Ok(ret.Result);
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

            return ret.Result is null ? NoContent() : (IActionResult)Ok(ret.Result);
        }
    }

        /// <summary>
    ///     Class DynamicQueryAllController. This class cannot be inherited.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// Implements the <see cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest}" />
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    [Route("api/[controller]")]
    [ApiController]
    public sealed class DynamicQueryAllController<TRequest> : ControllerBase,
        IDynamicController<TRequest>
    {
        private readonly IQueryHandler<TRequest, HandlerResponse> _getAllHandler;
        private readonly ILogger _log;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicQueryAllController{TRequest}"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="getAllHandler">The get all handler.</param>
        public DynamicQueryAllController(
            ILoggerFactory loggerFactory,
            IQueryHandler<TRequest, HandlerResponse> getAllHandler)
        {
            _log = loggerFactory.CreateLogger(GetType());
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

            return ret.Result is null ? NoContent() : (IActionResult)Ok(ValidateEnumerable(ret.Result));
        }

        private object ValidateEnumerable(object result)
        {
            var t = result.GetType();

            var enumerable = t.IsAssignableTo(typeof(IEnumerable)) && !t.IsAssignableTo(typeof(string));

            if (!enumerable)
                _log.LogWarning(() =>
                    $"GETALL controller being used at route '{Request.Path.Value}' with a result ({t.Name}) " +
                    "that does not appear to be enumerable.  Is this correct?"
                );

            return result;
        }
    }
}
