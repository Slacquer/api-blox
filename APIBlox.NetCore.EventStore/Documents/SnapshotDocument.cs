namespace APIBlox.NetCore.Documents
{
    internal class SnapshotDocument : EventStoreDocument
    {
        public override string Id => $"{StreamId}{Separator}{Version}{Separator}S";

        public override DocumentType DocumentType => DocumentType.Snapshot;
    }
}
