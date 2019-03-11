using System;
using System.Collections.Generic;
using System.Text;
using APIBlox.NetCore.Contracts;

namespace APIBlox.NetCore.Models
{
    public abstract class EventStoreDocumentBase: IEventStoreDocument
    {
        public EventStoreDocumentBase(string streamId, long timeStamp)
        {
            StreamId = streamId;
            TimeStamp = timeStamp;
        }

        public abstract string Id { get; }

        public string StreamId { get; private set; }

        public long TimeStamp { get; private set;}
    }
}
