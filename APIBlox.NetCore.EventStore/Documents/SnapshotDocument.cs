#region -    Using Statements    -

#endregion

namespace APIBlox.NetCore.Documents
{
    public class SnapshotDocument : DocumentBase
    {
        public override string Id => GenerateId(StreamId, Version);

        public override DocumentType DocumentType => DocumentType.Snapshot;

        public string SnapshotType { get; set; }

        public object SnapshotData { get; set; }

        public static string GenerateId(string streamId, ulong version)
        {
            return $"{streamId}{Separator}{version}{Separator}S";
        }
    }
}
