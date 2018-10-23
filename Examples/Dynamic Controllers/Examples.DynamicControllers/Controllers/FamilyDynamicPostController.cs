using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using Examples.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Examples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    internal sealed class FamilyDynamicPostController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TResponse : IResource<TId>, new()
    {
        private readonly IRandomNumberGeneratorService _rnd;

        public FamilyDynamicPostController(IRandomNumberGeneratorService randomNumberGeneratorService)
        {
            _rnd = randomNumberGeneratorService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(IResource), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public Task<IActionResult> Post(TRequest value, CancellationToken cancellationToken)
        {
            // Here we are just returning what came in, along with an id, more typically you
            // would end up calling some facade.  Less typically, you would have a command handler
            // taking care of all this for us.  And even more less typically, you could simple toss
            // this controller and use the one found in the CommandsQueriesControllersOhMy package :)
            var ret = new TResponse {Id = (TId) (object) _rnd.GenerateNumber(100)};

            JsonConvert.PopulateObject(JsonConvert.SerializeObject(value), ret);

            // We could use the createdAtRoute like below OR we could can return an ok as
            // the extension method in startup AddDynamicControllersFeature() has an optional
            // param to turn on (on by default) the postLocationHeaderFilter, which will
            // actually generate the location header from the result.
            // It can be kind of nice to not have to use yet another different action result.

            //      Be sure to notice the response headers in swashbuckle UI to see the location header.
            //
            //  WARNING: Using the postLocationHeaderFilter will always try to generate the route,
            //      even if you don't have a GET by id action.
            //
            //  I wonder what the CreatedAtRoute will do if you don't have a get by id?
            // Why don't you find out by setting the optional value to false in
            // the AddDynamicControllersFeature extension method.

            //return Task.FromResult((IActionResult) CreatedAtRoute(new {ret.Id}, ret));

            return Task.FromResult((IActionResult) Ok(ret));
        }
    }
}
