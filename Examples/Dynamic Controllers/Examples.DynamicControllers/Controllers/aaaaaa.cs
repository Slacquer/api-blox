//using APIBlox.AspNetCore.ActionResults;
//using APIBlox.AspNetCore.Contracts;
//using APIBlox.AspNetCore.Types;
//using Examples.Resources;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SomeNameSpace.Controllers
//{
//	/// <summary>
//    ///     Class FUllyDynamic.
//    /// </summary>
//    /// <typeparam name="DynamicControllerRequest">The controller actions request parameters.</typeparam>
//    /// <typeparam name="HandlerResponse">APIBlox default action response object</typeparam>
//    [Route("api/[controller]")]
//    [ApiController]
//    public sealed class FUllyDynamic : ControllerBase
//    {
//        private readonly IQueryHandler<DynamicControllerRequest, HandlerResponse> _getByHandler;

//		/// <summary>
//        ///     Initializes a new instance of the <see cref="FUllyDynamic" /> class.
//        /// </summary>
//        /// <param name="getAllHandler">The handler used to for querying DynamicControllerResponse.</param>
//        public FUllyDynamic(IQueryHandler<DynamicControllerRequest, HandlerResponse> getByHandler)
//        {
//            _getByHandler = getByHandler;
//        }

//		/// <summary>
//        ///     Action for getting a collection of DynamicControllerResponse resources.
//        ///     <para>
//        ///         Responses: 200, 204, 401, 403
//        ///     </para>
//        /// </summary>
//        /// <response code="200">Successful operation</response>
//        /// <response code="204">Success with no results</response>
//        /// <response code="401">UnAuthorized, meaning not authenticated.</response>
//        /// <response code="403">Authenticated yet not authorized</response>
//        [HttpGet]
//        [ProducesResponseType(typeof(DynamicControllerResponse), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        public async Task<IActionResult> QueryBy(
//            [FromQuery(Name = "requiredValueMustBeThreeCharacters")] String requiredValueMustBeThreeCharacters, [FromRoute(Name = "someId")] Int32 someId, [FromRoute(Name = "id")] Int32 id, 
//            CancellationToken cancellationToken
//        )
//        {
//            var ret = await _getByHandler.HandleAsync(
//                new DynamicControllerRequest{RequiredValueMustBeThreeCharacters = requiredValueMustBeThreeCharacters, SomeId = someId, Id = id}, 
//                cancellationToken
//            ).ConfigureAwait(false);

//            if (ret.HasErrors)
//                return new ProblemResult(ret.Error);
            
//            if (ret.Result is null)
//                throw new NullReferenceException(
//                    "When responding to a GET you must either set an error or pass a result!"
//                );

//            return Ok(ret.Result);
//        }
//    }
//}
