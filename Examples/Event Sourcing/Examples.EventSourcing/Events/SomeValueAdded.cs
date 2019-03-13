using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Examples.Events
{
    public class SomeValueAdded 
    {
        public SomeValueAdded(Guid aggregateId,  string someValue)
        {
            AggregateId = aggregateId;
            SomeValue = someValue;
        }
        

        public string SomeValue { get; }

        public Guid AggregateId { get; }
    }
}
