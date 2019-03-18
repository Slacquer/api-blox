using System.Linq;
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
        private readonly IEventStoreService<CosmosAggregate> _cosmosSvc;
        private readonly IEventStoreService<MongoAggregate> _mongoSvc;
        private readonly IEventStoreService<RavenAggregate> _ravenSvc;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventSourcingController" /> class.
        /// </summary>
        /// <param name="cosmosSvc">The cosmos SVC.</param>
        /// <param name="mongoSvc">The mongo SVC.</param>
        /// <param name="ravenSvc">The raven SVC.</param>
        public EventSourcingController(
            IEventStoreService<CosmosAggregate> cosmosSvc,
            IEventStoreService<MongoAggregate> mongoSvc,
            IEventStoreService<RavenAggregate> ravenSvc
        )
        {
            _cosmosSvc = cosmosSvc;
            _mongoSvc = mongoSvc;
            _ravenSvc = ravenSvc;
        }

        /// <summary>
        ///     Gets the specified first name.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpGet]
        public async Task<ActionResult> Get(string firstName)
        {
            var c = new CosmosAggregate(_cosmosSvc, firstName);
            var m = new MongoAggregate(_mongoSvc, firstName);
            var r = new RavenAggregate(_ravenSvc, Reverse(firstName));

            await Task.WhenAll(
                c.Build(true),
                m.Build(true),
                r.Build(true)
            );

            return Ok(new {CosmosAggregate = c, MongoAggregate = m, RavenAggregate = r});
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
            var c = new CosmosAggregate(_cosmosSvc, resource.FirstName);
            var m = new MongoAggregate(_mongoSvc, resource.FirstName);
            var r = new RavenAggregate(_ravenSvc, Reverse(resource.FirstName));

            await Task.WhenAll(
                c.AddSomeValue(resource.SomeValue, cancellationToken),
                m.AddSomeValue(resource.SomeValue, cancellationToken),
                r.AddSomeValue(Reverse(resource.SomeValue), cancellationToken)
            );

            await Task.WhenAll(
                c.PublishChangesAsync(cancellationToken),
                m.PublishChangesAsync(cancellationToken),
                r.PublishChangesAsync(cancellationToken)
            );

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
            var c = new CosmosAggregate(_cosmosSvc, firstName);
            var m = new MongoAggregate(_mongoSvc, firstName);
            var r = new RavenAggregate(_ravenSvc, Reverse(firstName));

            for (var i = 0; i < 10; i++)
            {
                await Task.WhenAll(
                    c.UpdateSomeValue(someValue + i, cancellationToken),
                    m.UpdateSomeValue(someValue + i, cancellationToken),
                    r.UpdateSomeValue(Reverse(someValue + i), cancellationToken)
                );

                await Task.WhenAll(
                    c.PublishChangesAsync(cancellationToken),
                    m.PublishChangesAsync(cancellationToken),
                    r.PublishChangesAsync(cancellationToken)
                );
            }

            return Accepted();
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
            var c = new CosmosAggregate(_cosmosSvc, firstName);
            var m = new MongoAggregate(_mongoSvc, firstName);
            var r = new RavenAggregate(_ravenSvc, Reverse(firstName));

            await Task.WhenAll(
                c.DeleteMe(cancellationToken),
                m.DeleteMe(cancellationToken),
                r.DeleteMe(cancellationToken)
            );

            return NoContent();
        }

        private static string Reverse(string str)
        {
            return string.Join("", str.Reverse());
        }
    }
}
