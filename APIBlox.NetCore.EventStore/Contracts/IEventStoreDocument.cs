using APIBlox.NetCore.Documents;

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Interface IEventStoreDocument
    /// </summary>
    public interface IEventStoreDocument
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        string Id { get; set; }

        /// <summary>
        ///     Gets or sets the type of the document.
        /// </summary>
        /// <value>The type of the document.</value>
        DocumentType DocumentType { get; set; }

        /// <summary>
        ///     Gets or sets the stream identifier.
        /// </summary>
        /// <value>The stream identifier.</value>
        string StreamId { get; set; }

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        long Version { get; set; }

        /// <summary>
        ///     Gets or sets the time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        long TimeStamp { get; set; }

        decimal SortOrder { get;  }
    }
}