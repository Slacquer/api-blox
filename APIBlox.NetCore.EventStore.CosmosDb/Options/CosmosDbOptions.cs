using System.Collections.Generic;

namespace APIBlox.NetCore.EventStore.CosmosDb
{
    public class CosmosDbOptions
    {
        public string Endpoint { get; set; }

        public string Key { get; set; }

        public string DatabaseId { get; set; }

        public Dictionary<string, Collection> Collections { get; set; } = new Dictionary<string, Collection>();
    }
}
