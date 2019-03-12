namespace APIBlox.NetCore.Models
{
    public class EventStreamModel
    {
        #region -    Constructors    -

        public EventStreamModel(string streamId, ulong version, ulong timeStamp, object metadata, EventModel[] events = null, SnapshotModel snapshot = null)
        {
            StreamId = streamId;
            Version = version;
            Metadata = metadata;
            Events = events;
            Snapshot = snapshot;
            TimeStamp = timeStamp;
        }

        #endregion

        public EventModel[] Events { get; }

        public object Metadata { get; }

        public SnapshotModel Snapshot { get; }

        public string StreamId { get; }

        public ulong Version { get; }

        public ulong TimeStamp { get; }
    }
}
