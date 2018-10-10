using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi2.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultDeleteByController<TRequest> : ControllerBase
    {
        private readonly ICommandHandler<TRequest, dynamic> _deleteByHandler;

        public DefaultDeleteByController(ICommandHandler<TRequest, dynamic> deleteByHandler)
        {
            _deleteByHandler = deleteByHandler;
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            var req = (TRequest) RouteData.Values[typeof(TRequest).Name];
            var ret = await _deleteByHandler.HandleAsync(req, cancellationToken).ConfigureAwait(false);

            return !(ret is null)
                ? (IActionResult) Ok(ret)
                : NotFound();
        }
    }
}
