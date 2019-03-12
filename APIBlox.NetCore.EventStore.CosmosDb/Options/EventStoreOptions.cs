namespace APIBlox.NetCore.Options
{
    public class EventStoreOptions : CosmosDbOptions
    {
        public string BulkInsertFilePath { get; set; }
    }
}
