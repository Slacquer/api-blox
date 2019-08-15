using APIBlox.NetCore.Documents;

namespace APIBlox.NetCore
{
    internal class DocEx : EventStoreDocument
    {
        public new string Data { get; set; }
    }
}
