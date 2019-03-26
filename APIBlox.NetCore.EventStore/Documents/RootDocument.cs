namespace APIBlox.NetCore.Documents
{
    internal class RootDocument : EventStoreDocument
    {
        public override string Id => StreamId;

        public override DocumentType DocumentType => DocumentType.Root;
        
    }
}
