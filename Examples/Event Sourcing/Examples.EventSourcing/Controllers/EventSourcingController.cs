using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using Examples.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    /// <summary>
    ///     Class EventSourcingController.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <inheritdoc />
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class EventSourcingController : ControllerBase
    {
        private readonly IEventStoreService<MyAggregate> _es;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventSourcingController" /> class.
        /// </summary>
        /// <param name="eventStoreService">The event store service.</param>
        public EventSourcingController(IEventStoreService<MyAggregate> eventStoreService)
        {
            _es = eventStoreService;
        }

        /// <summary>
        ///     Gets the specified first name.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpGet]
        public async Task<ActionResult> Get(string firstName)
        {
            var agg = new MyAggregate(_es, firstName);

            await agg.Build();

            return Ok(agg);
        }

        /// <summary>
        ///     Posts the specified resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPost]
        public async Task<ActionResult> Post(MyAggregateResource resource, CancellationToken cancellationToken)
        {
            var agg = new MyAggregate(_es, resource.FirstName);

            await agg.AddSomeValue(resource.SomeValue, cancellationToken);
            await agg.PublishChangesAsync(cancellationToken);

            return Accepted();
        }

        /// <summary>
        ///     Puts the specified first name.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="someValue">Some value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPut("{firstName}")]
        public async Task<ActionResult> Put(string firstName, [FromBody] string someValue, CancellationToken cancellationToken)
        {
            var agg = new MyAggregate(_es, firstName);

            await agg.UpdateSomeValue(someValue, cancellationToken);
            await agg.PublishChangesAsync(cancellationToken);

            return Accepted();
        }
    }
}
