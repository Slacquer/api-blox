using System;

namespace Examples.Events
{
    internal class SomeValueAdded
    {
        public SomeValueAdded(Guid aggregateId, string someValue)
        {
            AggregateId = aggregateId;
            SomeValue = someValue;
        }

        public string SomeValue { get; }

        public Guid AggregateId { get; }
    }
}
