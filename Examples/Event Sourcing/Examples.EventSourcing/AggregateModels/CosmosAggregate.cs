﻿using APIBlox.NetCore.Contracts;

namespace Examples.AggregateModels
{
    /// <inheritdoc />
    /// <summary>
    ///     Class CosmosAggregate.
    ///     Implements the <see cref="CosmosAggregate" />
    /// </summary>
    public class CosmosAggregate : Aggregate<CosmosAggregate>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CosmosAggregate" /> class.
        /// </summary>
        /// <param name="eventStoreService">The event store service.</param>
        /// <param name="streamId">The stream identifier.</param>
        public CosmosAggregate(IEventStoreService<CosmosAggregate> eventStoreService, string streamId)
            : base(eventStoreService, streamId)
        {
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Class CosmosAggregate.
    ///     Implements the <see cref="CosmosAggregate" />
    /// </summary>
    public class CosmosAggregate2 : Aggregate<CosmosAggregate2>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CosmosAggregate2" /> class.
        /// </summary>
        /// <param name="eventStoreService">The event store service.</param>
        /// <param name="streamId">The stream identifier.</param>
        public CosmosAggregate2(IEventStoreService<CosmosAggregate2> eventStoreService, string streamId)
            : base(eventStoreService, streamId)
        {
        }
    }
}
