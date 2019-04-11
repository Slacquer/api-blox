using APIBlox.NetCore.Contracts;

namespace Examples.AggregateModels
{
    /// <inheritdoc />
    /// <summary>
    ///     Class MongoAggregate.
    ///     Implements the <see cref="MongoAggregate" />
    /// </summary>
    public class MongoAggregate : Aggregate<MongoAggregate>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MongoAggregate" /> class.
        /// </summary>
        /// <param name="eventStoreService">The event store service.</param>
        /// <param name="streamId">The stream identifier.</param>
        public MongoAggregate(IEventStoreService<MongoAggregate> eventStoreService, string streamId)
            : base(eventStoreService, streamId)
        {
        }
    }
}
