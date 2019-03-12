using System;
using System.Collections.Generic;
using System.Text;

namespace APIBlox.NetCore.Contracts
{
    public interface IEventStoreDocument
    {
        string Id { get; }

        string StreamId { get; }

        long TimeStamp { get; }
    }
}
