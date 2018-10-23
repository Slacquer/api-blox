using System.Collections.Generic;
using System.Threading;
using APIBlox.AspNetCore.Contracts;
using Examples.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    internal sealed class ChildrenDynamicGetAllController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TRequest : IChildRequest
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

            // This kid must like candy so do something else with the result.

            return Ok(req);
        }
    }
}
