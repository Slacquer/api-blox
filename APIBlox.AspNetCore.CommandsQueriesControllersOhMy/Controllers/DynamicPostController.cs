using System;
using System.Linq;
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
    ///     Class DynamicPostController.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <typeparam name="TResponse">The type of the t response.</typeparam>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest, TResponse, TId}" />
    [Route("api/[controller]")]
    [ApiController]
    public sealed class DynamicPostController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TRequest : class
        where TResponse : IResource<TId>
    {
        private readonly ICommandHandler<TRequest, HandlerResponse> _createCommand;
        private readonly ILogger<DynamicPostController<TRequest, TResponse, TId>> _log;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicPostController{TRequest, TResponse, TId}" /> class.
        /// </summary>
        /// <param name="loggerFactory">LoggerFactory</param>
        /// <param name="createCommand">The create command.</param>
        public DynamicPostController(ILoggerFactory loggerFactory, ICommandHandler<TRequest, HandlerResponse> createCommand)
        {
            _createCommand = createCommand;
            _log = loggerFactory.CreateLogger<DynamicPostController<TRequest, TResponse, TId>>();
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
        public async Task<IActionResult> Post([FromRoute] TRequest value, CancellationToken cancellationToken)
        {
            var ret = await _createCommand.HandleAsync(value, cancellationToken).ConfigureAwait(false);

            var errorResult = ret.HasErrors
                ? new ProblemResult(ret.Error)
                : null;

            if (errorResult is null && ret.Result is null)
                throw new NullReferenceException(
                    "When responding to a POST you must either set an error or pass some results!"
                );

            if (!(errorResult is null))
                return errorResult;

            var id = FindId(ret.Result);

            return id == -1
                ? (IActionResult)Ok(ret.Result)
                : (IActionResult)CreatedAtRoute(new { id }, ret.Result);
        }

        // But it seems sometimes responses do not match what was
        // requested, I think that's a violation but what do I know...
        private object FindId(object result)
        {
            try
            {
                var t = result.GetType();
                var props = t.GetProperties();

                var id = props.FirstOrDefault(p => p.Name.EqualsEx("Id"));

                if (id is null)
                {
                    foreach (var pi in props)
                        return FindId(t.GetProperty(pi.Name).GetValue(result, null));
                }
                else
                    return t.GetProperty(id.Name).GetValue(result, null);
            }
            catch (Exception ex)
            {
                _log.LogError(() => $"Could not determine ID for CreatedAtRoute result!  Ex: {ex.Message}");

                return -1;
            }

            return -1;
        }
    }
}
