namespace APIBlox.NetCore.Documents
{
    internal class SnapshotDocument : EventStoreDocument
    {
        public override string Id => $"{StreamId}-{Version}-S";

        public override DocumentType DocumentType => DocumentType.Snapshot;
    }
}
