namespace APIBlox.NetCore.EventStore.Options
{
    /// <summary>
    ///     Class CollectionProperties.
    /// </summary>
    public class CollectionProperties
    {
        /// <summary>
        ///     Gets or sets the unique keys.
        /// </summary>
        /// <value>The unique keys.</value>
        public string[] UniqueKeys { get; set; } = new string[0];

        /// <summary>
        ///     Gets or sets the offer throughput.
        /// </summary>
        /// <value>The offer throughput.</value>
        public int OfferThroughput { get; set; } = 400;
    }
}
