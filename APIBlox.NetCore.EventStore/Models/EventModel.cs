using System.Diagnostics;

namespace APIBlox.NetCore.Models
{
    [DebuggerDisplay("Version:{Version}")]
    public class EventModel
    {
        #region -    Constructors    -

        public EventModel(object data, ulong version,  ulong timeStamp, object metadata)
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

        #endregion

        public object Data { get; }
        public object Metadata { get; }
        public ulong Version { get; }
        public ulong TimeStamp { get; }
    }
}
