namespace APIBlox.NetCore.Documents
{
    public class RootDocument : EventStoreDocument
    {
        public override string Id => GenerateId(StreamId);

        public override DocumentType DocumentType => DocumentType.Root;

        public static string GenerateId(string streamId)
        {
            return streamId;
        }
    }
}
