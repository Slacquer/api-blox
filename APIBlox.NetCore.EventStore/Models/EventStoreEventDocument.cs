using System;
using System.Collections.Generic;
using System.Text;

namespace APIBlox.NetCore.Models
{
    public class EventStoreEventDocument:EventStoreDocumentBase
    {
        public EventStoreEventDocument(string streamId, long index, long timeStamp, object eventData)
            : base(streamId, timeStamp)
        {
            EventData = eventData;
            EventType = eventData.GetType().Name;
            Index = index;
        }

        public override string Id => $"{StreamId}-{Index}";

        public long Index { get; }

        public string EventType { get; private set; }

        public object EventData { get; private set; }
    }
}
