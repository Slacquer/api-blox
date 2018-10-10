using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.RequestsResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Controllers
{
    /// <summary>
    ///     Class DynamicPutController.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest}" />
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicPutController<TRequest> : ControllerBase,
        IDynamicController<TRequest>
        where TRequest : class
    {
        private readonly ICommandHandler<TRequest, HandlerResponse> _putHandler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicPutController{TRequest}" /> class.
        /// </summary>
        /// <param name="putHandler">The put handler.</param>
        public DynamicPutController(ICommandHandler<TRequest, HandlerResponse> putHandler)
        {
            _putHandler = putHandler;
        }

        /// <summary>
        ///     Action for updating a resource.
        ///     <para>
        ///         Responses: 204, 401, 403, 404, 409
        ///     </para>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public virtual async Task<IActionResult> Put(TRequest value, CancellationToken cancellationToken)
        {
            //  If a service does not support UPSERT, then a PATCH/PUT call against a resource that
            //  does not exist MUST result in an HTTP "409 Conflict" error.
            var ret = await _putHandler.HandleAsync(value, cancellationToken).ConfigureAwait(false);

            return ret.HasErrors
                ? (IActionResult) new ProblemResult(ret.Error)
                : NoContent();
        }
    }
}
