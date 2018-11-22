using System;
using APIBlox.NetCore.Contracts;

namespace Examples.EventBits
{
    /// <summary>
    ///     Class RequestObjectCreatedEvent.
    /// </summary>
    /// <seealso cref="APIBlox.NetCore.Contracts.IDomainEvent" />
    public class RequestObjectCreatedEvent : IDomainEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RequestObjectCreatedEvent" /> class.
        /// </summary>
        /// <param name="theValueThatWasCreated">The value that was created.</param>
        /// <param name="someOtherDomainSpecificEventValueNeededForConsumption">
        ///     Some other domain specific event value needed for consumption.
        /// </param>
        public RequestObjectCreatedEvent(int theValueThatWasCreated, string someOtherDomainSpecificEventValueNeededForConsumption)
        {
            AggregateId = new Random().Next();

            TheValueThatWasCreated = theValueThatWasCreated;
            SomeOtherDomainSpecificEventValueNeededForConsumption = someOtherDomainSpecificEventValueNeededForConsumption;
        }

        /// <summary>
        ///     Gets the aggregate identifier.
        /// </summary>
        public int AggregateId { get; }

        /// <summary>
        ///     Gets the value that was created.
        /// </summary>
        /// <value>The value that was created.</value>
        public int TheValueThatWasCreated { get; }

        /// <summary>
        ///     Gets some other domain specific event value needed for consumption.
        /// </summary>
        public string SomeOtherDomainSpecificEventValueNeededForConsumption { get; }
    }
}
