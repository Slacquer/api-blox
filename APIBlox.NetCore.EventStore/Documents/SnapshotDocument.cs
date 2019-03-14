namespace APIBlox.NetCore.Documents
{
    public class SnapshotDocument : EventStoreDocument
    {
        public override string Id => GenerateId(StreamId, Version);

        public override DocumentType DocumentType => DocumentType.Snapshot;
        
        public static string GenerateId(string streamId, long version)
        {
            return $"{streamId}{Separator}{version}{Separator}S";
        }
    }
}
