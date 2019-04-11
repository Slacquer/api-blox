using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using Examples.AggregateModels;
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
    public class EventSourcingController<TAggregate> : ControllerBase
        where TAggregate : Aggregate<TAggregate>
    {
        private readonly IEventStoreService<TAggregate> _svc;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventSourcingRavenDbController" /> class.
        /// </summary>
        /// <param name="svc">The SVC.</param>
        public EventSourcingController(
            IEventStoreService<TAggregate> svc
        )
        {
            _svc = svc;
        }

        /// <summary>
        ///     Gets the specified first name.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpGet]
        public async Task<ActionResult> Get(string firstName)
        {
            var m = new Aggregate<TAggregate>(_svc, firstName);

            await m.Build(true);

            return Ok(m);
        }

        /// <summary>
        ///     Posts the specified resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPost]
        public async Task<ActionResult> Post(AggregateResource resource, CancellationToken cancellationToken)
        {
            var m = new Aggregate<TAggregate>(_svc, resource.FirstName);

            await m.AddSomeValue(resource.SomeValue, cancellationToken);
            await m.PublishChangesAsync(cancellationToken);

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
            var m = new Aggregate<TAggregate>(_svc, firstName);

            await m.UpdateSomeValue(someValue, cancellationToken);
            await m.PublishChangesAsync(cancellationToken);

            return NoContent();
        }

        /// <summary>
        ///     Deletes the specified first name.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpDelete("{firstName}")]
        public async Task<ActionResult> Delete(string firstName, CancellationToken cancellationToken)
        {
            var m = new Aggregate<TAggregate>(_svc, firstName);

            await m.DeleteMe(cancellationToken);

            return NoContent();
        }
    }
}
