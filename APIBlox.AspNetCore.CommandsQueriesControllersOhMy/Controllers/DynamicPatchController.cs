#region -    Using Statements    -

using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.CommandQueryResponses;
using APIBlox.AspNetCore.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace APIBlox.AspNetCore.Controllers
{
    /// <summary>
    ///     Class DynamicPatchController.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest}" />
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicPatchController<TRequest> : ControllerBase,
        IDynamicController<TRequest>
        where TRequest : class
    {
        #region -    Fields    -

        private readonly IPatchCommandHandler<TRequest, HandlerResponse> _patchHandler;

        private readonly string _rn = typeof(TRequest).Name;

        #endregion

        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicPatchController{TRequest}" /> class.
        /// </summary>
        /// <param name="patchHandler">The patch handler.</param>
        public DynamicPatchController(IPatchCommandHandler<TRequest, HandlerResponse> patchHandler)
        {
            _patchHandler = patchHandler;
        }

        #endregion

        /// <summary>
        ///     Action for patching a resource.
        ///     <para>
        ///         Responses: 204, 401, 403, 404, 409
        ///     </para>
        /// </summary>
        /// <param name="patch">The patch.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public virtual async Task<IActionResult> Patch(
            JsonPatchDocument<TRequest> patch,
            CancellationToken cancellationToken
        )
        {
            //  If a service does not support UPSERT, then a PATCH/PUT call against a resource that
            //  does not exist MUST result in an HTTP "409 Conflict" error.
            var req = (TRequest) RouteData.Values[_rn];

            var ret = await _patchHandler.HandleAsync(req, patch, cancellationToken).ConfigureAwait(false);

            return ret.HasErrors
                ? (IActionResult) new ProblemResult(ret.Error)
                : NoContent();
        }
    }
}
