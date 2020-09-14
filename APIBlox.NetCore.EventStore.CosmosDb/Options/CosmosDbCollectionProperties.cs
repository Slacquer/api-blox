using System.Collections.Generic;
using Microsoft.Azure.Documents;

namespace APIBlox.NetCore.Options
{
    /// <summary>
    ///     Class CollectionProperties.
    /// </summary>
    public class CosmosDbCollectionProperties
    {
        /// <summary>
        ///     Gets or sets the models.
        /// </summary>
        public List<string> Models { get; set; }

        /// <summary>
        ///     Gets or sets the unique key policy.
        /// </summary>
        public UniqueKeyPolicy UniqueKeyPolicy { get; set; } = new UniqueKeyPolicy();

        /// <summary>
        ///     Gets or sets the offer throughput.
        /// </summary>
        public int OfferThroughput { get; set; } = -1;
    }
}
