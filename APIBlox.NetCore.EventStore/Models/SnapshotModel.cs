namespace APIBlox.NetCore.Models
{
    public class SnapshotModel
    {
        public SnapshotModel(object data, object metadata, long version, long timeStamp)
        {
            Data = data;
            Metadata = metadata;
            Version = version;
            TimeStamp = timeStamp;
        }

        public object Data { get; }
        public object Metadata { get; }
        public long Version { get; }

        public long TimeStamp { get;  }
    }
}
