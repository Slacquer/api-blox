namespace APIBlox.NetCore.Documents
{
    internal class EventDocument : EventStoreDocument
    {
        public override string Id => $"{StreamId}{Separator}{Version}";

        public override DocumentType DocumentType => DocumentType.Event;
    }
}
