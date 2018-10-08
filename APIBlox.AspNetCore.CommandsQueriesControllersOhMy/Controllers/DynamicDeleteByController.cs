#region -    Using Statements    -

using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.RequestsResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace APIBlox.AspNetCore.Controllers
{
    /// <summary>
    ///     Class DynamicDeleteByController.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IDynamicController{TRequest}" />
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicDeleteByController<TRequest> : ControllerBase,
        IDynamicController<TRequest>
        where TRequest : class
    {
        #region -    Fields    -

        private readonly ICommandHandler<TRequest, HandlerResponse> _deleteByHandler;

        private readonly string _rn = typeof(TRequest).Name;

        #endregion

        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicDeleteByController{TRequest}" /> class.
        /// </summary>
        /// <param name="deleteByHandler">The delete by handler.</param>
        public DynamicDeleteByController(ICommandHandler<TRequest, HandlerResponse> deleteByHandler)
        {
            _deleteByHandler = deleteByHandler;
        }

        #endregion

        /// <summary>
        ///     Action for deleting a resource.
        ///     <para>
        ///         Responses: 204, 401, 403, 404
        ///     </para>
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            var req = (TRequest) RouteData.Values[_rn];
            var ret = await _deleteByHandler.HandleAsync(req, cancellationToken).ConfigureAwait(false);

            return ret.HasErrors
                ? (IActionResult) new ProblemResult(ret.Error)
                : NoContent();
        }
    }
}
