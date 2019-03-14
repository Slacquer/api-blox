namespace APIBlox.NetCore.EventStore.MongoDb.Options
{
    public class Collection
    {
        public string Id { get; set; }
        
        public string[] UniqueKeys { get; set; } = new string[0];
    }
}
