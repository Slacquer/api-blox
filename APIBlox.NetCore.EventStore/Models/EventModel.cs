using System.Diagnostics;

namespace APIBlox.NetCore.Models
{
    [DebuggerDisplay("Version:{Version}")]
    public class EventModel
    {
        public EventModel(object data, long version,  long timeStamp, object metadata)
        {
            Data = data;
            Metadata = metadata;
            Version = version;
            TimeStamp = timeStamp;
        }

        public EventModel(object data, object metadata = null)
        {
            Data = data;
            Metadata = metadata;
        }

        public object Data { get; }
        public object Metadata { get; }
        public long Version { get; }
        public long TimeStamp { get; }
    }
}
