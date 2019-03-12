namespace APIBlox.NetCore.Models
{
    public class SnapshotModel
    {
        #region -    Constructors    -

        public SnapshotModel(object data, object metadata, ulong version, ulong timeStamp)
        {
            Data = data;
            Metadata = metadata;
            Version = version;
            TimeStamp = timeStamp;
        }

        #endregion

        public object Data { get; }
        public object Metadata { get; }
        public ulong Version { get; }

        public ulong TimeStamp { get;  }
    }
}
