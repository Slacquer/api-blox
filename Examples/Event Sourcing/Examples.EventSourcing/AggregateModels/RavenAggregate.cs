using APIBlox.NetCore.Contracts;

namespace Examples.AggregateModels
{
    /// <inheritdoc />
    /// <summary>
    ///     Class RavenAggregate.
    ///     Implements the <see cref="RavenAggregate" />
    /// </summary>
    public class RavenAggregate : Aggregate<RavenAggregate>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RavenAggregate" /> class.
        /// </summary>
        /// <param name="eventStoreService">The event store service.</param>
        /// <param name="streamId">The stream identifier.</param>
        public RavenAggregate(IEventStoreService<RavenAggregate> eventStoreService, string streamId)
            : base(eventStoreService, streamId)
        {
        }
    }
}
