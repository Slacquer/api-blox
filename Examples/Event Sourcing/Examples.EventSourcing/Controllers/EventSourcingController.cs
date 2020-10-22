using System.Collections.Generic;
using System.Linq;
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpGet("{firstName}")]
        public async Task<ActionResult> Get([FromRoute]string firstName, CancellationToken cancellationToken)
        {
            var m = new Aggregate<TAggregate>(_svc, firstName);

            await m.BuildAsync(true, cancellationToken);

            return Ok(m);
        }

        /// <summary>
        ///     Gets all entries.
        /// </summary>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpGet]
        public async Task<ActionResult> Get(CancellationToken cancellationToken)
        {
            var streams = await _svc.ReadEventStreamsAsync(null, cancellationToken: cancellationToken);

            var lst = new List<Aggregate<TAggregate>>();
            foreach (var es in streams)
            {
                var m = new Aggregate<TAggregate>(_svc, es.StreamId);

                await m.BuildAsync(true, cancellationToken);

                lst.Add(m);

            }

            return Ok(lst.ToList());
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

            await m.AddSomeValueAsync(resource.SomeValue, cancellationToken);
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

            await m.UpdateSomeValueAsync(someValue, cancellationToken);
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

            await m.DeleteMeAsync(cancellationToken);

            return NoContent();
        }
    }
}
