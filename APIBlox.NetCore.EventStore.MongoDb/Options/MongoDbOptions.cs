using System.Collections.Generic;

namespace APIBlox.NetCore.EventStore.MongoDb.Options
{
    public class MongoDbOptions
    {
        public string ConnectionString { get; set; }
        
        public string DatabaseId { get; set; }
    }
}
