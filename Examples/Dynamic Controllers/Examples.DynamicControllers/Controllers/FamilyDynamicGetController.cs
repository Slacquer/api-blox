using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
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
        public ActionResult GetAll([FromRoute] TRequest request,CancellationToken cancellationToken)
        {
            // The action filter will fill some things in for us when we use
            // the extension method .AddPopulateGenericRequestObjectActionFilter()
            
            var lst = new List<TResponse>();

            for (var i = 0; i < 30; i++)
                lst.Add(new TResponse());

            return Ok(lst);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IResource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Get([FromRoute] TRequest request,CancellationToken cancellationToken)
        {
            var ret = new TResponse();

            JsonConvert.PopulateObject(JsonConvert.SerializeObject(request), ret);

            return Task.FromResult((IActionResult) Ok(ret));
        }
    }
}
