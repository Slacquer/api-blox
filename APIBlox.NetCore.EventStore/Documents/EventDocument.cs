namespace APIBlox.NetCore.Documents
{
    internal class EventDocument : EventStoreDocument
    {
        public override string Id => $"{StreamId}-{Version}";

        public override DocumentType DocumentType => DocumentType.Event;
    }
}
