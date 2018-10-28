using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using Examples.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Examples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    internal sealed class FamilyDynamicGetController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TResponse : IResource<TId>, new()
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult GetAll(CancellationToken cancellationToken)
        {
            // The action filter will fill some things in for us when we use
            // the extension method .AddPopulateGenericRequestObjectActionFilter()
            var req = (TRequest) RouteData.Values[typeof(TRequest).Name];

            var parent = req as ParentRequest;

            return parent is null ? Ok(req as ChildRequest) : Ok(parent);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var req = (TRequest) RouteData.Values[typeof(TRequest).Name];

            var ret = new TResponse();

            JsonConvert.PopulateObject(JsonConvert.SerializeObject(req), ret);

            return Task.FromResult((IActionResult) Ok(ret));
        }
    }
}
