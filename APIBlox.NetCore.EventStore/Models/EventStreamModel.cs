namespace APIBlox.NetCore.Models
{
    public class EventStreamModel
    {
        public EventStreamModel(string streamId, long version, long timeStamp, object metadata, EventModel[] events = null, SnapshotModel snapshot = null)
        {
            StreamId = streamId;
            Version = version;
            Metadata = metadata;
            Events = events;
            Snapshot = snapshot;
            TimeStamp = timeStamp;
        }

        public EventModel[] Events { get; }

        public object Metadata { get; }

        public SnapshotModel Snapshot { get; }

        public string StreamId { get; }

        public long Version { get; }

        public long TimeStamp { get; }
    }
}
