using System.Collections.Generic;

namespace APIBlox.NetCore.Options
{
    /// <summary>
    ///     Class RavenDbOptions.
    /// </summary>
    public class RavenDbOptions
    {
        /// <summary>
        ///     Gets or sets the database identifier.
        /// </summary>
        /// <value>The database identifier.</value>
        public string DatabaseId { get; set; }

        /// <summary>
        ///     Gets or sets the collection properties.
        /// </summary>
        /// <value>The collection properties.</value>
        public Dictionary<string, RavenDbCollectionProperties> CollectionProperties { get; set; } =
            new Dictionary<string, RavenDbCollectionProperties>();

        /// <summary>
        ///     Gets or sets the urls.
        /// </summary>
        /// <value>The urls.</value>
        public string[] Urls { get; set; }
    }
}
