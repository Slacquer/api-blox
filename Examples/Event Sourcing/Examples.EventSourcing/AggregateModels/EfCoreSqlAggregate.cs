using APIBlox.NetCore.Contracts;

namespace Examples.AggregateModels
{
    /// <inheritdoc />
    /// <summary>
    ///     Class EfCoreSqlAggregate.
    ///     Implements the <see cref="EfCoreSqlAggregate" />
    /// </summary>
    public class EfCoreSqlAggregate : Aggregate<EfCoreSqlAggregate>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EfCoreSqlAggregate" /> class.
        /// </summary>
        /// <param name="eventStoreService">The event store service.</param>
        /// <param name="streamId">The stream identifier.</param>
        public EfCoreSqlAggregate(IEventStoreService<EfCoreSqlAggregate> eventStoreService, string streamId)
            : base(eventStoreService, streamId)
        {
        }
    }
}
