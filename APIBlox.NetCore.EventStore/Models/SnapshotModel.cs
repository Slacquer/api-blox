namespace APIBlox.NetCore.Models
{
    public class SnapshotModel
    {
        public object Data { get; set;}

        public string DataType { get; set; }

        public object Metadata { get;set; }

        public string MetadataType { get;set; }

        public long Version { get; set;}
    }
}
