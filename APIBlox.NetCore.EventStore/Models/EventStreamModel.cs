namespace APIBlox.NetCore.Models
{
    public class EventStreamModel
    {
        public EventModel[] Events { get; set;}

        public object Metadata { get; set;}

        public SnapshotModel Snapshot { get; set;}

        public string StreamId { get; set;}

        public long Version { get; set;}

        public long TimeStamp { get; set;}
    }
}
