//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using APIBlox.AspNetCore.ActionResults;
//using APIBlox.AspNetCore.Contracts;
//using APIBlox.AspNetCore.Types;
//using APIBlox.NetCore.Extensions;
//using Examples.Resources;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;

//namespace Examples.Controllers
//{
//    /// <inheritdoc />
//    /// <summary>
//    ///     Class Children.
//    /// </summary>
//    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
//    [Route("api/[controller]/{someRouteValueWeNeed}/parents/{parentId}/children")]
//    [ApiController]
//    public sealed class Children : ControllerBase
//    {
//        private readonly ICommandHandler<ChildByIdRequest, HandlerResponse> _deleteByChildByIdRequestHandler;
//        private readonly IQueryHandler<ChildrenRequest, HandlerResponse> _getAllChildResponseHandler;
//        private readonly IQueryHandler<ChildByIdRequest, HandlerResponse> _getByChildResponseHandler;
//        private readonly ILogger<Children> _log;
//        private readonly ICommandHandler<ChildPostRequest, HandlerResponse> _postByChildPostRequestHandler;
//        private readonly ICommandHandler<ChildPutRequest, HandlerResponse> _putByChildPutRequestHandler;

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="Children" /> class.
//        /// </summary>
//        /// <param name="getByChildResponseHandler">The handler used to for querying ChildResponse.</param>
//        public Children(IQueryHandler<ChildByIdRequest, HandlerResponse> getByChildResponseHandler)
//        {
//            _getByChildResponseHandler = getByChildResponseHandler;
//        }

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="Children" /> class.
//        /// </summary>
//        /// <param name="deleteByChildByIdRequestHandler">The handler used to for deleting a resource.</param>
//        public Children(ICommandHandler<ChildByIdRequest, HandlerResponse> deleteByChildByIdRequestHandler)
//        {
//            _deleteByChildByIdRequestHandler = deleteByChildByIdRequestHandler;
//        }

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="Children" /> class.
//        /// </summary>
//        /// <param name="getAllChildResponseHandler">The handler used to for querying ChildResponse.</param>
//        public Children(IQueryHandler<ChildrenRequest, HandlerResponse> getAllChildResponseHandler)
//        {
//            _getAllChildResponseHandler = getAllChildResponseHandler;
//        }

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="Children" /> class.
//        /// </summary>
//        /// <param name="putByChildPutRequestHandler">The handler used to for updating a resource.</param>
//        public Children(ICommandHandler<ChildPutRequest, HandlerResponse> putByChildPutRequestHandler)
//        {
//            _putByChildPutRequestHandler = putByChildPutRequestHandler;
//        }

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="Children" /> class.
//        /// </summary>
//        /// <param name="loggerFactory">LoggerFactory</param>
//        /// <param name="postByChildPostRequestHandler">The handler used to for creating a resource.</param>
//        public Children(ILoggerFactory loggerFactory, ICommandHandler<ChildPostRequest, HandlerResponse> postByChildPostRequestHandler)
//        {
//            _postByChildPostRequestHandler = postByChildPostRequestHandler;
//            _log = loggerFactory.CreateLogger<Children>();
//        }

//        /// <summary>
//        ///     Action for getting a ChildResponse resource.
//        /// </summary>
//        /// <remarks>
//        ///     Possible Response Status Codes: <a href="https://httpstatuses.com/200">200</a>,
//        ///     <a href="https://httpstatuses.com/204">204</a>, <a href="https://httpstatuses.com/401">401</a>,
//        ///     <a href="https://httpstatuses.com/403">403</a>
//        /// </remarks>
//        /// <response code="200">Success, with a result.</response>
//        /// <response code="204">Success, no results.</response>
//        /// <response code="401">
//        ///     Unauthorized, You are not authenticated, meaning not authenticated at all or authenticated
//        ///     incorrectly.
//        /// </response>
//        /// <response code="403">
//        ///     Forbidden, You have successfully been authenticated, yet you do not have permission
//        ///     (authorization) to access the requested resource.
//        /// </response>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <param name="id">Sets the child id.</param>
//        /// <param name="parentId">Gets or sets the parent identifier.</param>
//        /// <param name="someRouteValueWeNeed">Gets or sets some route value we need.</param>
//        [HttpGet("{childId}")]
//        [Produces("application/json")]
//        [Consumes("application/json")]
//        [ProducesResponseType(typeof(ChildResponse), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        public async Task<IActionResult> QueryBy([FromRoute(Name = "childId")] int id, [FromRoute] int parentId, [FromRoute] int someRouteValueWeNeed,
//            CancellationToken cancellationToken
//        )
//        {
//            var ret = await _getByChildResponseHandler
//                .HandleAsync(new ChildByIdRequest {Id = id, ParentId = parentId, SomeRouteValueWeNeed = someRouteValueWeNeed}, cancellationToken)
//                .ConfigureAwait(false);
//            if (ret.HasErrors)
//                return new ProblemResult(ret.Error);

//            if (ret.Result is null)
//                throw new ArgumentNullException(nameof(HandlerResponse.Result),
//                    "When responding to a GET you must either set an error or pass a result!"
//                );

//            return Ok(ret.Result);
//        }

//        /// <summary>
//        ///     Action for deleting a resource.
//        /// </summary>
//        /// <remarks>
//        ///     Possible Response Status Codes: <a href="https://httpstatuses.com/204">204</a>,
//        ///     <a href="https://httpstatuses.com/401">401</a>, <a href="https://httpstatuses.com/403">403</a>,
//        ///     <a href="https://httpstatuses.com/404">404</a>
//        /// </remarks>
//        /// <response code="204">Success, no results.</response>
//        /// <response code="401">
//        ///     Unauthorized, You are not authenticated, meaning not authenticated at all or authenticated
//        ///     incorrectly.
//        /// </response>
//        /// <response code="403">
//        ///     Forbidden, You have successfully been authenticated, yet you do not have permission
//        ///     (authorization) to access the requested resource.
//        /// </response>
//        /// <response code="404">NotFound, The resource was not found using the supplied input parameters.</response>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <param name="id">Sets the child id.</param>
//        /// <param name="parentId">Gets or sets the parent identifier.</param>
//        /// <param name="someRouteValueWeNeed">Gets or sets some route value we need.</param>
//        [HttpDelete("{childId}")]
//        [Produces("application/json")]
//        [Consumes("application/json")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> DeleteBy([FromRoute(Name = "childId")] int id, [FromRoute] int parentId,
//            [FromRoute] int someRouteValueWeNeed, CancellationToken cancellationToken
//        )
//        {
//            var ret = await _deleteByChildByIdRequestHandler
//                .HandleAsync(new ChildByIdRequest {Id = id, ParentId = parentId, SomeRouteValueWeNeed = someRouteValueWeNeed}, cancellationToken)
//                .ConfigureAwait(false);
//            if (ret.HasErrors)
//                return new ProblemResult(ret.Error);

//            return ret.HasErrors ? (IActionResult) new ProblemResult(ret.Error) : NoContent();
//        }

//        /// <summary>
//        ///     Action for getting a collection of ChildResponse resources.
//        /// </summary>
//        /// <remarks>
//        ///     Possible Response Status Codes: <a href="https://httpstatuses.com/200">200</a>,
//        ///     <a href="https://httpstatuses.com/204">204</a>, <a href="https://httpstatuses.com/401">401</a>,
//        ///     <a href="https://httpstatuses.com/403">403</a>
//        /// </remarks>
//        /// <response code="200">Success, with an array of results.</response>
//        /// <response code="204">Success, no results.</response>
//        /// <response code="401">
//        ///     Unauthorized, You are not authenticated, meaning not authenticated at all or authenticated
//        ///     incorrectly.
//        /// </response>
//        /// <response code="403">
//        ///     Forbidden, You have successfully been authenticated, yet you do not have permission
//        ///     (authorization) to access the requested resource.
//        /// </response>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <param name="parentId">Gets or sets the parent identifier.</param>
//        /// <param name="someRouteValueWeNeed">Gets or sets some route value we need.</param>
//        /// <param name="filter">Sets the filter (where). Usage is determined by the API itself.</param>
//        /// <param name="select">Sets the select (projection).  Usage is determined by the API itself.</param>
//        /// <param name="orderBy">Sets the order by.  Usage is determined by the API itself.</param>
//        [HttpGet]
//        [Produces("application/json")]
//        [Consumes("application/json")]
//        [ProducesResponseType(typeof(IEnumerable<ChildResponse>), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        public async Task<IActionResult> QueryAll([FromRoute] int parentId, [FromRoute] int someRouteValueWeNeed,
//            [FromQuery(Name = "filter")] string filter, [FromQuery(Name = "select")] string select, [FromQuery(Name = "orderBy")] string orderBy,
//            CancellationToken cancellationToken
//        )
//        {
//            var ret = await _getAllChildResponseHandler.HandleAsync(new ChildrenRequest
//                    {ParentId = parentId, SomeRouteValueWeNeed = someRouteValueWeNeed, Filter = filter, Select = select, OrderBy = orderBy},
//                cancellationToken
//            ).ConfigureAwait(false);
//            if (ret.HasErrors)
//                return new ProblemResult(ret.Error);

//            if (ret.Result is null)
//                throw new ArgumentNullException(nameof(HandlerResponse.Result),
//                    "When responding to a GET you must either set an error or pass a result!"
//                );

//            return Ok(ret.Result);
//        }

//        /// <summary>
//        ///     Action for updating a resource.
//        /// </summary>
//        /// <remarks>
//        ///     Possible Response Status Codes: <a href="https://httpstatuses.com/204">204</a>,
//        ///     <a href="https://httpstatuses.com/401">401</a>, <a href="https://httpstatuses.com/403">403</a>,
//        ///     <a href="https://httpstatuses.com/404">404</a>, <a href="https://httpstatuses.com/409">409</a>
//        /// </remarks>
//        /// <response code="204">Success, no results.</response>
//        /// <response code="401">
//        ///     Unauthorized, You are not authenticated, meaning not authenticated at all or authenticated
//        ///     incorrectly.
//        /// </response>
//        /// <response code="403">
//        ///     Forbidden, You have successfully been authenticated, yet you do not have permission
//        ///     (authorization) to access the requested resource.
//        /// </response>
//        /// <response code="404">NotFound, The resource was not found using the supplied input parameters.</response>
//        /// <response code="409">Conflict, The supplied input parameters would cause a data violation.</response>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <param name="id">Sets the child id.</param>
//        /// <param name="parentId">Gets or sets the parent identifier.</param>
//        /// <param name="anQueryValueWeWant">Gets or sets an query value we want.</param>
//        /// <param name="body">Gets or sets the body.</param>
//        [HttpPut("{childId}")]
//        [Produces("application/json")]
//        [Consumes("application/json")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        [ProducesResponseType(StatusCodes.Status409Conflict)]
//        public async Task<IActionResult> PutBy([FromRoute(Name = "childId")] int id, [FromRoute(Name = "parentId")] int parentId,
//            [FromQuery] [Required(AllowEmptyStrings = false)]
//            int anQueryValueWeWant, [FromBody] PersonModel body, CancellationToken cancellationToken
//        )
//        {
//            var ret = await _putByChildPutRequestHandler
//                .HandleAsync(new ChildPutRequest {Id = id, ParentId = parentId, AnQueryValueWeWant = anQueryValueWeWant, Body = body},
//                    cancellationToken
//                ).ConfigureAwait(false);
//            return ret.HasErrors ? (IActionResult) new ProblemResult(ret.Error) : NoContent();
//        }

//        /// <summary>
//        ///     Action for creating a resource.
//        /// </summary>
//        /// <remarks>
//        ///     Possible Response Status Codes: <a href="https://httpstatuses.com/204">204</a>,
//        ///     <a href="https://httpstatuses.com/401">401</a>, <a href="https://httpstatuses.com/403">403</a>,
//        ///     <a href="https://httpstatuses.com/404">404</a>, <a href="https://httpstatuses.com/409">409</a>
//        /// </remarks>
//        /// <response code="204">Success, no results.</response>
//        /// <response code="401">
//        ///     Unauthorized, You are not authenticated, meaning not authenticated at all or authenticated
//        ///     incorrectly.
//        /// </response>
//        /// <response code="403">
//        ///     Forbidden, You have successfully been authenticated, yet you do not have permission
//        ///     (authorization) to access the requested resource.
//        /// </response>
//        /// <response code="404">NotFound, The resource was not found using the supplied input parameters.</response>
//        /// <response code="409">Conflict, The supplied input parameters would cause a data violation.</response>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <param name="parentId">Gets or sets the parent identifier.</param>
//        /// <param name="body">Gets or sets the body.</param>
//        [HttpPost]
//        [Produces("application/json")]
//        [Consumes("application/json")]
//        [ProducesResponseType(typeof(ChildResponse), StatusCodes.Status201Created)]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status403Forbidden)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        [ProducesResponseType(StatusCodes.Status409Conflict)]
//        public async Task<IActionResult> Post([FromRoute(Name = "parentId")] int parentId, [FromBody] PersonModel body,
//            CancellationToken cancellationToken
//        )
//        {
//            var ret = await _postByChildPostRequestHandler.HandleAsync(new ChildPostRequest {ParentId = parentId, Body = body}, cancellationToken)
//                .ConfigureAwait(false);
//            var errorResult = ret.HasErrors ? new ProblemResult(ret.Error) : null;
//            if (errorResult is null && ret.Result is null)
//                throw new NullReferenceException("When responding to a POST you must either set an error or pass some results!");

//            if (!(errorResult is null))
//                return errorResult;

//            var id = FindId(ret.Result);
//            return Equals(id, -1)
//                ? Ok(ret.Result)
//                : CreatedAtRoute(new
//                    {
//                        id
//                    },
//                    ret.Result
//                );
//        }

//        private object FindId(object result)
//        {
//            try
//            {
//                var t = result.GetType();
//                var props = t.GetProperties();
//                var id = props.FirstOrDefault(p => p.Name.EqualsEx("Id"));
//                if (id is null)
//                    foreach (var pi in props)
//                        return FindId(t.GetProperty(pi.Name).GetValue(result, null));
//                else
//                    return t.GetProperty(id.Name).GetValue(result, null);
//            }
//            catch (Exception ex)
//            {
//                _log.LogError(() => $"Could not determine ID for CreatedAtRoute result!  Ex: {ex.Message}");
//                return -1;
//            }

//            return -1;
//        }
//    }
//}
