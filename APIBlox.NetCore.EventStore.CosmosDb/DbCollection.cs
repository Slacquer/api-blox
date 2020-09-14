using System;
using APIBlox.NetCore.Options;

namespace APIBlox.NetCore
{
    internal class DbCollection
    {
        public int DatabaseThroughput { get; set; }

        public string DatabaseId { get; set; }

        public string CollectionId { get; set; }

        public Uri DocumentCollectionUri { get; set; }

        public CosmosDbCollectionProperties ColProps { get; set; }
    }
}
