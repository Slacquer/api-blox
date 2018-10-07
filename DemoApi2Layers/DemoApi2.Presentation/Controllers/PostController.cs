#region -    Using Statements    -

using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DemoApi2.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TResponse : IResource<TId>
    {
        #region -    Fields    -

        private readonly ICommandHandler<TRequest, dynamic> _createCommand;

        #endregion

        #region -    Constructors    -

        public PostController(ICommandHandler<TRequest, dynamic> createCommand)
        {
            _createCommand = createCommand;
        }

        #endregion

        [HttpPost]
        [ProducesResponseType(typeof(IResource), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, TRequest request)
        {
            var result = await _createCommand.HandleAsync(request, cancellationToken).ConfigureAwait(false);

            return CreatedAtRoute(new {result.Id}, result);
        }
    }
}
