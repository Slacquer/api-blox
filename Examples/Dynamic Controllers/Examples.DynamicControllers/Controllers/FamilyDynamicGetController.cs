using System.Collections.Generic;
using System.Threading;
using APIBlox.AspNetCore.Contracts;
using Examples.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    internal class FamilyDynamicGetController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TResponse : IResource<TId>
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

            if (parent is null)
            {
                var child = req as ChildRequest;
            }

            return Ok();
        }
    }
}
