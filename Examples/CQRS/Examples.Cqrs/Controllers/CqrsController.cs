using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Contracts;
using Examples.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Very simple usage examples for our CQRS bits.  More real world usage can be found in the Clean Architecture
    ///     example.
    ///     <para>
    ///         I think the big "Take away" from these examples is to realize just how easy it ends up being to test the
    ///         handlers rather than controller code.  They are self contained small blocks of testable code. (or at least
    ///         should/could be :)
    ///     </para>
    ///     <para>
    ///         FYI: (my opinion anyways...)
    ///     </para>
    ///     <para>
    ///         Request Validation = "Anything that CAN be tested without the need for external sources (IE: checking a db
    ///         value)."
    ///     </para>
    ///     <para>
    ///         Domain Validation = "Anything that CAN NOT be tested without the need for external sources (IE: checking a db
    ///         value)."
    ///     </para>
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/resources/[controller]")]
    [ApiController]
    public class CqrsController : ControllerBase
    {
        private readonly ICommandHandler<ExampleRequestObject, HandlerResponse> _commandHandler;
        private readonly IQueryHandler<int, int> _queryInputsHandler;
        private readonly IQueryHandler<IEnumerable<string>> _queryNoInputsHandler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CqrsController" /> class.
        /// </summary>
        /// <param name="queryNoInputsHandler">The query no inputs handler.</param>
        /// <param name="queryInputsHandler">The query inputs handler.</param>
        /// <param name="commandHandler">The command handler.</param>
        public CqrsController(
            IQueryHandler<IEnumerable<string>> queryNoInputsHandler,
            IQueryHandler<int, int> queryInputsHandler,
            ICommandHandler<ExampleRequestObject, HandlerResponse> commandHandler
        )
        {
            _queryNoInputsHandler = queryNoInputsHandler;
            _queryInputsHandler = queryInputsHandler;
            _commandHandler = commandHandler;
        }

        /// <summary>
        ///     An example of a query handler that doesn't require inputs.
        /// </summary>
        /// <returns>ActionResult&lt;IEnumerable&lt;System.String&gt;&gt;.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            return Ok(await _queryNoInputsHandler.HandleAsync(CancellationToken.None));
        }

        /// <summary>
        ///     An example of a query handler that has inputs.
        /// </summary>
        /// <returns>ActionResult&lt;IEnumerable&lt;System.String&gt;&gt;.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _queryInputsHandler.HandleAsync(id, CancellationToken.None));
        }

        /// <summary>
        ///     An example of a command handler, that is also "wrapped" with a decorator (a great use case is being able to do some
        ///     domain validation before letting the handler deal with it).
        /// </summary>
        /// <param name="requestResource">The request resource.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public async Task<ActionResult> Post(ExampleRequestObject requestResource)
        {
           var ret = await _commandHandler.HandleAsync(requestResource, CancellationToken.None);

           if (ret.HasErrors)
               return new ProblemResult(ret.Error);

            return Ok();
        }
    }
}
