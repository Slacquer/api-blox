using System.Collections.Generic;

namespace APIBlox.NetCore.EventStore.Options
{
    /// <summary>
    ///     Class CosmosDbOptions.
    /// </summary>
    public class CosmosDbOptions
    {
        /// <summary>
        ///     Gets or sets the endpoint.
        /// </summary>
        /// <value>The endpoint.</value>
        public string Endpoint { get; set; }

        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets the database identifier.
        /// </summary>
        /// <value>The database identifier.</value>
        public string DatabaseId { get; set; }

        /// <summary>
        ///     Gets or sets the collection properties.
        /// </summary>
        /// <value>The collection properties.</value>
        public Dictionary<string, CosmosDbCollectionProperties> CollectionProperties { get; set; } = new Dictionary<string, CosmosDbCollectionProperties>();
    }
}
