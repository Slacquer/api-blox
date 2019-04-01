//using System.Threading;
//using System.Threading.Tasks;
//using APIBlox.AspNetCore.ActionResults;
//using APIBlox.AspNetCore.Contracts;
//using APIBlox.AspNetCore.Types;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.JsonPatch;
//using Microsoft.AspNetCore.Mvc;

//namespace APIBlox.AspNetCore.Controllers
//{
//    /// <summary>
//    ///     Class DynamicPatchController.
//    /// </summary>
//    /// <typeparam name="TPatchRequest">The type of the t request.</typeparam>
//    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
//    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest}" />
//    [Route("api/[controller]")]
//    [ApiController]
//    public sealed class DynamicPatchController<TPatchRequest> : ControllerBase,
//        IDynamicController<TPatchRequest>
//    {
//        private readonly IPatchCommandHandler<TPatchRequest, HandlerResponse> _patchHandler;

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="DynamicPatchController{TRequest}" /> class.
//        /// </summary>
//        /// <param name="patchHandler">The patch handler.</param>
//        public DynamicPatchController(IPatchCommandHandler<TPatchRequest, HandlerResponse> patchHandler)
//        {
//            _patchHandler = patchHandler;
//        }

//        /// <summary>
//        ///     Action for patching a resource.
//        ///     <para>
//        ///         Responses: 204, 401, 403, 404, 409
//        ///     </para>
//        /// </summary>
//        /// <param name="request">The request.</param>
//        /// <param name="patch">The patch.</param>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <returns>Task&lt;IActionResult&gt;.</returns>
//        [HttpPatch]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        [ProducesResponseType(StatusCodes.Status409Conflict)]
//        public async Task<IActionResult> Patch(
//            [FromRoute] TPatchRequest request,
//            JsonPatchDocument patch,
//            CancellationToken cancellationToken
//        )
//        {
//            var ret = await _patchHandler.HandleAsync(request, patch, cancellationToken).ConfigureAwait(false);

//            return ret.HasErrors
//                ? (IActionResult) new ProblemResult(ret.Error)
//                : NoContent();
//        }
//    }
//}
