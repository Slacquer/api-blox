using System;
using System.Collections.Generic;
using System.Text;

namespace APIBlox.NetCore.Contracts
{
    public interface IEventStoreDocument
    {
        string StreamId { get; }

        long TimeStamp { get; }
    }
}
