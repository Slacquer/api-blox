using System.Collections.Generic;

namespace APIBlox.NetCore.EventStore.Options
{
    /// <summary>
    ///     Class MongoDbOptions.
    /// </summary>
    public class MongoDbOptions
    {
        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string CnnString { get; set; }

        /// <summary>
        ///     Gets or sets the database identifier.
        /// </summary>
        /// <value>The database identifier.</value>
        public string DatabaseId { get; set; }

        /// <summary>
        ///     Gets or sets the collection properties.
        /// </summary>
        /// <value>The collection properties.</value>
        public Dictionary<string, CollectionProperties> CollectionProperties { get; set; } = new Dictionary<string, CollectionProperties>();
    }
}
