using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Controllers
{
    /// <summary>
    ///     Class DynamicGenericPatchController.
    /// </summary>
    /// <typeparam name="TPatchRequest">The type of the t request.</typeparam>
    /// <typeparam name="TPatchObject">The patch object</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest}" />
    [Route("api/[controller]")]
    [ApiController]
    public sealed class DynamicGenericPatchController<TPatchRequest, TPatchObject> : ControllerBase,
        IDynamicController<TPatchRequest>
        where TPatchRequest : PatchRequest<TPatchObject>
        where TPatchObject : class
    {
        private readonly IPatchCommandHandler<TPatchRequest, TPatchObject, HandlerResponse> _patchHandler;

        private readonly string _rn = typeof(TPatchRequest).Name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicPatchController{TRequest}" /> class.
        /// </summary>
        /// <param name="patchHandler">The patch handler.</param>
        public DynamicGenericPatchController(IPatchCommandHandler<TPatchRequest, TPatchObject, HandlerResponse> patchHandler)
        {
            _patchHandler = patchHandler;
        }

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
        public async Task<IActionResult> Patch(
            [FromRoute] JsonPatchDocument<TPatchObject> patch,
            CancellationToken cancellationToken
        )
        {
            //  If a service does not support UPSERT, then a PATCH/PUT call against a resource that
            //  does not exist MUST result in an HTTP "409 Conflict" error.
            var req = (TPatchRequest) RouteData.Values[_rn];

            req.Patch = patch;

            var ret = await _patchHandler.HandleAsync(req, cancellationToken).ConfigureAwait(false);

            return ret.HasErrors
                ? (IActionResult) new ProblemResult(ret.Error)
                : NoContent();
        }
    }
}
