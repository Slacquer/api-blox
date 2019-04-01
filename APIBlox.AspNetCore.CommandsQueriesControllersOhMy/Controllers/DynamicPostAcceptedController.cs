//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using APIBlox.AspNetCore.ActionResults;
//using APIBlox.AspNetCore.Contracts;
//using APIBlox.AspNetCore.Types;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace APIBlox.AspNetCore.Controllers
//{
//    /// <summary>
//    ///     Class DynamicPostAcceptedController.
//    /// </summary>
//    /// <typeparam name="TRequest">The type of the t request.</typeparam>
//    /// <typeparam name="TResponse">The type of the t response.</typeparam>
//    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
//    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest, TResponse}" />
//    [Route("api/[controller]")]
//    [ApiController]
//    public sealed class DynamicPostAcceptedController<TRequest, TResponse> : ControllerBase,
//        IDynamicController<TRequest, TResponse>
//        where TRequest : class
//        where TResponse : IResource
//    {
//        private readonly ICommandHandler<TRequest, HandlerResponse> _createCommand;

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="DynamicPostAcceptedController{TRequest, TResponse}" /> class.
//        /// </summary>
//        /// <param name="createCommand">The create command.</param>
//        public DynamicPostAcceptedController(ICommandHandler<TRequest, HandlerResponse> createCommand)
//        {
//            _createCommand = createCommand;
//        }

//        /// <summary>
//        ///     Action for creating a resource but will return an Accepted status code (202).
//        ///     <para>
//        ///         Responses: 202, 401, 403, 404, 409
//        ///     </para>
//        /// </summary>
//        /// <param name="value">The value.</param>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <returns>Task&lt;IActionResult&gt;.</returns>
//        [HttpPost]
//        [ProducesResponseType(typeof(IResource), StatusCodes.Status202Accepted)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        [ProducesResponseType(StatusCodes.Status409Conflict)]
//        public async Task<IActionResult> Post(TRequest value, CancellationToken cancellationToken)
//        {
//            var ret = await _createCommand.HandleAsync(value, cancellationToken).ConfigureAwait(false);

//            var errorResult = ret.HasErrors
//                ? new ProblemResult(ret.Error)
//                : null;

//            if (errorResult is null && ret.Result is null)
//                throw new NullReferenceException(
//                    "When responding to a POST you must either set an error or pass some results!"
//                );

//            if (!(errorResult is null))
//                return errorResult;

//            return (IActionResult) Accepted(ret.Result);
//        }
//    }
//}
