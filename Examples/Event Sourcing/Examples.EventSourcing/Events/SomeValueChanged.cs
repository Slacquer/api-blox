using System;

namespace Examples.Events
{
    internal class SomeValueChanged
    {
        public SomeValueChanged(Guid aggregateId, string someValue)
        {
            AggregateId = aggregateId;
            SomeValue = someValue;
        }

        public string SomeValue { get; }

        public Guid AggregateId { get; }
    }
}
