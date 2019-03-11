using System;
using System.Collections.Generic;
using System.Text;

namespace APIBlox.NetCore.Models
{
    public class EventStoreStreamRootDocument : EventStoreDocumentBase
    {
        public EventStoreStreamRootDocument(string streamId, long version, long timeStamp)
            : base(streamId, timeStamp)
        {
            Version = version;
        }

        public override string Id => StreamId;

        public long Version { get;  set; }
        
    }
}
