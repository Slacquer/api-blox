namespace APIBlox.NetCore.Documents
{
    public class EventDocument : EventStoreDocument
    {
        public override string Id => GenerateId(StreamId, Version);

        public override DocumentType DocumentType => DocumentType.Event;

        public string EventType { get; set; }

        public object EventData { get; set; }

        public static string GenerateId(string streamId, long version)
        {
            return $"{streamId}{Separator}{version}";
        }
    }
}
