using System.Collections.Generic;
using Microsoft.Azure.Documents.Client;

namespace APIBlox.NetCore.Options
{
    /// <summary>
    ///     Class CosmosDbOptions.
    /// </summary>
    public class CosmosDbOptions
    {
        /// <summary>
        ///     Gets or sets the endpoint.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets the database identifier.
        /// </summary>
        public string DatabaseId { get; set; }

        /// <summary>
        ///     Gets or sets the connection policy.
        /// </summary>
        public ConnectionPolicy ConnectionPolicy { get; set; }

        /// <summary>
        ///     Gets or sets the offer throughput.
        /// </summary>
        public int OfferThroughput { get; set; } = 400;

        /// <summary>
        ///     Gets or sets the collection properties.
        /// </summary>
        public Dictionary<string, CosmosDbCollectionProperties> CollectionProperties { get; set; } =
            new Dictionary<string, CosmosDbCollectionProperties>();
    }
}
