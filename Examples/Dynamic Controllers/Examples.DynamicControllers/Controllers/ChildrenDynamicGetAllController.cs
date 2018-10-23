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
            
            // keep in mind the route had the LikesCandy value (specified in our route template in config),
            // and APIBlox filled the TRequest for us, but it's worth noting that the TRequest
            // didn't HAVE to have a LikesCandy property for you to get the value,
            // it just helps when passing this model around if it is already filled in.

            // This kid must like candy so do something else with the result.

            return Ok(req);
        }
    }
}
