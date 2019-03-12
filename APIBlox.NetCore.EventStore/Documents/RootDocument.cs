namespace APIBlox.NetCore.Documents
{
    internal class RootDocument : DocumentBase
    {
        public override string Id => GenerateId(StreamId);

        public override DocumentType DocumentType => DocumentType.Root;

        public static string GenerateId(string streamId)
        {
            return streamId;
        }
    }
}
