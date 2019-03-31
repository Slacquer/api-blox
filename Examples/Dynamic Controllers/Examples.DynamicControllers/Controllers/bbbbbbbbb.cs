//using APIBlox.AspNetCore.ActionResults;
//using APIBlox.AspNetCore.Contracts;
//using APIBlox.AspNetCore.Types;
//using Examples.Resources;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Examples.Controllers
//{
//    /// <inheritdoc />
//    /// <summary>
//    ///     Class Foo.
//    /// </summary>
//    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
//    [Route("api/[controller]")]
//    [ApiController]
//    public sealed class Foo : ControllerBase
//    {
//        private readonly IQueryHandler<ParentRequest, HandlerResponse> _getAllHandler;

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="Foo" /> class.
//        /// </summary>
//        /// <param name="getAllHandler">The handler used to for querying ParentResponse.</param>
//        public Foo(IQueryHandler<ParentRequest, HandlerResponse> getAllHandler)
//        {
//            _getAllHandler = getAllHandler;
//        }

//		/// <summary>
//        ///     Action for getting a collection of ParentResponse resources.
//        /// </summary>
//        /// <remarks>
//        ///     Possible Response Status Codes: <a href="https://httpstatuses.com/200">200</a>, <a href="https://httpstatuses.com/204">204</a>, <a href="https://httpstatuses.com/401">401</a>, <a href="https://httpstatuses.com/403">403</a>
//        /// </remarks>
//        /// <response code="200">Success, with an array of results.</response>
//        /// <response code="204">Success, no results.</response>
//        /// <response code="401">Unauthorized, You are not authenticated, meaning not authenticated at all or authenticated incorrectly.</response>
//        /// <response code="403">Forbidden, You have successfully been authenticated, yet you do not have permission (authorization) to access the requested resource.</response>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <param name ="someOtherThingWeNeedToKnowWhenRequestingData">Gets or sets some other thing we need to know when requesting data.</param>
//        /// <param name ="someRouteValueWeNeed">Gets or sets some route value we need.</param>
//        /// <param name ="filter">Sets the filter (where). Usage is determined by the API itself.</param>
//        /// <param name ="select">Sets the select (projection).  Usage is determined by the API itself.</param>
//        /// <param name ="runningCount">Gets the running count.  Used internally.</param>
//        /// <param name ="skip">Sets the skip amount.</param>
//        /// <param name ="top">Sets the top amount.</param>
//        /// <param name ="orderBy">Sets the order by.  Usage is determined by the API itself.</param>
//        [HttpGet]
//        [Produces("application/json")]
//        [Consumes("application/json")]
//        [ProducesResponseType(typeof(IEnumerable<ParentResponse>), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        public async Task<IActionResult> QueryAll(
//            [Required(AllowEmptyStrings = false)][FromQuery] String someOtherThingWeNeedToKnowWhenRequestingData,
//            [FromRoute(Name = "someRouteValueWeNeed")] Int32 someRouteValueWeNeed,
//            [FromQuery(Name = "filter")] String filter,
//            [FromQuery(Name = "select")] String select,
//            [FromQuery(Name = "runningCount")] Int32? runningCount,
//            [FromQuery(Name = "skip")] Int32? skip,
//            [FromQuery(Name = "top")] Int32? top,
//            [FromQuery(Name = "orderBy")] String orderBy,
//            CancellationToken cancellationToken
//        )
//        {
//            var ret = await _getAllHandler.HandleAsync(
//                new ParentRequest{SomeOtherThingWeNeedToKnowWhenRequestingData = someOtherThingWeNeedToKnowWhenRequestingData, SomeRouteValueWeNeed = someRouteValueWeNeed, Filter = filter, Select = select, RunningCount = runningCount, Skip = skip, Top = top, OrderBy = orderBy}, 
//                cancellationToken
//            ).ConfigureAwait(false);

//            if (ret.HasErrors)
//                return new ProblemResult(ret.Error);

//            if (ret.Result is null)
//                throw new ArgumentNullException(
//                    nameof(HandlerResponse.Result),
//                    "When responding to a GET you must either set an error or pass a result!"
//                );

//            return Ok(ret.Result);
//        }
//    }
//}
