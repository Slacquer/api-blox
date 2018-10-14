using System;
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
    ///     Class DynamicPostController.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <typeparam name="TResponse">The type of the t response.</typeparam>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest, TResponse, TId}" />
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicPostController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TRequest : class
        where TResponse : IResource<TId>
    {
        private readonly ICommandHandler<TRequest, HandlerResponse> _createCommand;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicPostController{TRequest, TResponse, TId}" /> class.
        /// </summary>
        /// <param name="createCommand">The create command.</param>
        public DynamicPostController(ICommandHandler<TRequest, HandlerResponse> createCommand)
        {
            _createCommand = createCommand;
        }

        /// <summary>
        ///     Action for creating a resource.
        ///     <para>
        ///         Responses: 201, 401, 403, 404, 409
        ///     </para>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(IResource), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public virtual async Task<IActionResult> Post(TRequest value, CancellationToken cancellationToken)
        {
            var ret = await _createCommand.HandleAsync(value, cancellationToken).ConfigureAwait(false);

            var errorResult = ret.HasErrors
                ? new ProblemResult(ret.Error)
                : null;

            if (errorResult is null && ret.Result is null)
                throw new NullReferenceException("When responding to a POST you must either set an error or pass some results!");

            return errorResult ?? CreatedAtRoute(new {ret.Result.Id}, ret.Result);
        }
    }
}
