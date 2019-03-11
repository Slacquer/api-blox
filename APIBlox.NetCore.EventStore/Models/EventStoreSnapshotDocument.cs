using System;
using System.Collections.Generic;
using System.Text;

namespace APIBlox.NetCore.Models
{
    public class EventStoreSnapshotDocument : EventStoreDocumentBase
    {
        public EventStoreSnapshotDocument(string streamId, long timeStamp, object snapshotData)
            : base(streamId, timeStamp)
        {
            SnapshotData = snapshotData;
        }

        public override string Id => $"{StreamId}-{Index}-S";

        public long Index { get; private set; }
        

        public object SnapshotData { get; private set; }
    }
}
