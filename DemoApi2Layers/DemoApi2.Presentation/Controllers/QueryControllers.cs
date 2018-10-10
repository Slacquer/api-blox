using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.RequestsResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi2.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TResponse : IResource<TId>
    {
        private readonly IQueryHandler<TRequest, IEnumerable<dynamic>> _getAllHandler;

        private readonly string _reqName = typeof(TRequest).Name;

        public QueryController(IQueryHandler<TRequest, IEnumerable<dynamic>> getAllHandler)
        {
            _getAllHandler = getAllHandler;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var req = (TRequest) RouteData.Values[_reqName];
            var result = (await _getAllHandler.HandleAsync(req, cancellationToken).ConfigureAwait(false)).ToList();

            return !result.Any()
                ? (IActionResult) NoContent()
                : Ok(result);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class QueryByIdController<TRequest> : ControllerBase
    {
        private readonly IQueryHandler<TRequest, HandlerResponse> _getByIdHandler;

        private readonly string _reqName = typeof(TRequest).Name;

        public QueryByIdController(IQueryHandler<TRequest, HandlerResponse> getByIdHandler)
        {
            _getByIdHandler = getByIdHandler;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var req = (TRequest) RouteData.Values[_reqName];
            var ret = await _getByIdHandler.HandleAsync(req, cancellationToken).ConfigureAwait(false);

            return ret.HasErrors
                ? new ProblemResult(ret.Error)
                : Ok(ret.Result);
        }
    }
}
