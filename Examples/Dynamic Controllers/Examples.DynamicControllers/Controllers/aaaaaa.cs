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
//    ///     Class MyComposedController.
//    /// </summary>
//    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
//    [Route("api/[controller]")]
//    [ApiController]
//    public sealed class MyComposedController : ControllerBase
//    {
//        private readonly IQueryHandler<DynamicControllerRequest, HandlerResponse> _getAllHandler;

//		/// <summary>
//        ///     Initializes a new instance of the <see cref="MyComposedController" /> class.
//        /// </summary>
//        /// <param name="getAllHandler">The handler used to for querying DynamicControllerResponse.</param>
//        public MyComposedController(IQueryHandler<DynamicControllerRequest, HandlerResponse> getAllHandler)
//        {
//            _getAllHandler = getAllHandler;
//        }

//		/// <summary>
//        ///     Action for getting a collection of DynamicControllerResponse resources.
//        /// </summary>
//        /// <remarks>
//        ///     Possible Response Status Codes: <a href="https://httpstatuses.com/200">200</a>, <a href="https://httpstatuses.com/204">204</a>, <a href="https://httpstatuses.com/401">401</a>, <a href="https://httpstatuses.com/403">403</a>
//        /// </remarks>
//        /// <response code="200">Success, with an array of results.</response>
//        /// <response code="204">Success, no results.</response>
//        /// <response code="401">Unauthorized, You are not authenticated, meaning not authenticated at all or authenticated incorrectly.</response>
//        /// <response code="403">Forbidden, You do not have permission to access the requested DynamicControllerRequest resource.</response>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <param name ="requiredValueMustBeThreeCharacters">The required value, and it must be three characters. please check out http://www.foo.com for more info.</param>
//        /// <param name ="someId">Gets or sets some identifier.</param>
//        /// <param name ="id">Gets or sets the identifier.</param>
//        [HttpGet]
//		[Produces("application/json")]
//		[Consumes("application/json")]
//        [ProducesResponseType(typeof(IEnumerable<DynamicControllerResponse>), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        public async Task<IActionResult> QueryAll(
//            [FromQuery(Name = "requiredValueMustBeThreeCharacters")][Required(AllowEmptyStrings = false)][StringLength(3, MinimumLength = 3, ErrorMessage = "Value must be exactly 3 characters long.")] String requiredValueMustBeThreeCharacters,
//            [FromRoute(Name = "someId")] Int32 someId,
//            [FromRoute(Name = "id")] Int32 id,
//            CancellationToken cancellationToken
//        )
//        {
//            var ret = await _getAllHandler.HandleAsync(
//                new DynamicControllerRequest{RequiredValueMustBeThreeCharacters = requiredValueMustBeThreeCharacters, SomeId = someId, Id = id}, 
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
