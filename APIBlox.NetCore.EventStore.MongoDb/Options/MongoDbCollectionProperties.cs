namespace APIBlox.NetCore.EventStore.Options
{
    /// <summary>
    ///     Class MongoDbCollectionProperties.
    /// </summary>
    public class MongoDbCollectionProperties
    {
        /// <summary>
        ///     Gets or sets the indexes.
        /// </summary>
        /// <value>The indexes.</value>
        public string[] Indexes { get; set; } = new string[0];
    }
}
