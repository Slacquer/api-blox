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
//    ///     Class QueryByChildResponseController.
//    /// </summary>
//    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
//    [Route("api/[controller]/{someRouteValueWeNeed}/parents/{parentId}/children")]
//    [ApiController]
//    public sealed class QueryByChildResponseController : ControllerBase
//    {

//        /// <summary>
//        ///     Action for getting a ChildResponse resource.
//        /// </summary>
//        /// <remarks>
//        ///     Possible Response Status Codes: <a href="https://httpstatuses.com/200">200</a>, <a href="https://httpstatuses.com/204">204</a>, <a href="https://httpstatuses.com/401">401</a>, <a href="https://httpstatuses.com/403">403</a>
//        /// </remarks>
//        /// <response code="200">Success, with a result.</response>
//        /// <response code="204">Success, no results.</response>
//        /// <response code="401">Unauthorized, You are not authenticated, meaning not authenticated at all or authenticated incorrectly.</response>
//        /// <response code="403">Forbidden, You have successfully been authenticated, yet you do not have permission (authorization) to access the requested resource.</response>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <param name ="id">Sets the child id.</param>
//        /// <param name ="parentId">Gets or sets the parent identifier.</param>
//        /// <param name ="someRouteValueWeNeed">Gets or sets some route value we need.</param>
//        [HttpGet("{childId}")]
//        [Produces("application/json")]
//        [Consumes("application/json")]
//        [ProducesResponseType(typeof(ChildResponse), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        public async Task<IActionResult> QueryBy(
//            [FromRoute(Name = "childId")] Int32 id,
//            [FromRoute(Name = "parentId")] Int32 parentId,
//            [FromRoute(Name = "someRouteValueWeNeed")] Int32 someRouteValueWeNeed,
//			CancellationToken cancellationToken
//        )
//        {
//            return Ok();
//        }
//    }
//}
