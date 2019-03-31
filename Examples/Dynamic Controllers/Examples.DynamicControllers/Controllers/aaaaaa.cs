//using APIBlox.AspNetCore.ActionResults;
//using APIBlox.AspNetCore.Contracts;
//using APIBlox.AspNetCore.Types;
//using Examples.Resources;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Examples.Controllers
//{
//    /// <inheritdoc />
//    /// <summary>
//    ///     Class PutUsingChildPutRequestController.
//    /// </summary>
//    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
//    [Route("api/[controller]/{someRouteValueWeNeed}/parents/{parentId}/children")]
//    [ApiController]
//    public sealed class PutUsingChildPutRequestController : ControllerBase
//    {
//        private readonly ICommandHandler<ChildPutRequest, HandlerResponse> _putByHandler;

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="PutUsingChildPutRequestController" /> class.
//        /// </summary>
//        /// <param name="putByHandler">The handler used to for updating a resource.</param>
//        public PutUsingChildPutRequestController(ICommandHandler<ChildPutRequest, HandlerResponse> putByHandler)
//        {
//            _putByHandler = putByHandler;
//        }

//        /// <summary>
//        ///     Action for updating a resource.
//        /// </summary>
//        /// <remarks>
//        ///     Possible Response Status Codes: <a href="https://httpstatuses.com/204">204</a>, <a href="https://httpstatuses.com/401">401</a>, <a href="https://httpstatuses.com/403">403</a>, <a href="https://httpstatuses.com/404">404</a>, <a href="https://httpstatuses.com/409">409</a>
//        /// </remarks>
//        /// <response code="204">Success, no results.</response>
//        /// <response code="401">Unauthorized, You are not authenticated, meaning not authenticated at all or authenticated incorrectly.</response>
//        /// <response code="403">Forbidden, You have successfully been authenticated, yet you do not have permission (authorization) to access the requested resource.</response>
//        /// <response code="404">NotFound, The resource was not found using the supplied input parameters.</response>
//        /// <response code="409">Conflict, The supplied input parameters would cause a data violation.</response>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <param name ="id">Sets the child id.</param>
//        /// <param name ="parentId">Gets or sets the parent identifier.</param>
//        /// <param name ="someRouteValueWeNeed">Gets or sets some route value we need.</param>
//        /// <param name ="age"></param>
//        /// <param name ="name"></param>
//        [HttpPut("{childId}")]
//        [Produces("application/json")]
//        [Consumes("application/json")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        [ProducesResponseType(StatusCodes.Status409Conflict)]
//        public async Task<IActionResult> PutBy(
//            [FromRoute(Name = "childId")] Int32 id,
//            [FromRoute(Name = "parentId")] Int32 parentId,
//            [FromRoute(Name = "someRouteValueWeNeed")] Int32 someRouteValueWeNeed,
//            [FromBody()] Int32 age,
//            [FromBody()] String name,
//			CancellationToken cancellationToken
//        )
//        {
//            var ret = await _putByHandler.HandleAsync(
//                new ChildPutRequest{Id = id, ParentId = parentId, SomeRouteValueWeNeed = someRouteValueWeNeed, Age = age, Name = name}, 
//                cancellationToken
//            ).ConfigureAwait(false);

//            return ret.HasErrors
//                ? (IActionResult) new ProblemResult(ret.Error)
//                : NoContent();
//        }
//    }
//}
