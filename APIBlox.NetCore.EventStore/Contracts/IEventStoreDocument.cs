#region -    Using Statements    -

using APIBlox.NetCore.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#endregion

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Interface IEventStoreDocument
    /// </summary>
    public interface IEventStoreDocument
    {
        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [JsonProperty(PropertyName = "id")]
        string Id { get; }

        /// <summary>
        ///     Gets the stream identifier.
        /// </summary>
        /// <value>The stream identifier.</value>
        [JsonProperty(PropertyName = "streamId")]
        string StreamId { get; }

        /// <summary>
        ///     Gets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        [JsonProperty(PropertyName = "sortOrder")]
        decimal SortOrder { get; }

        /// <summary>
        ///     Gets the type of the document.
        /// </summary>
        /// <value>The type of the document.</value>
        [JsonProperty(PropertyName = "documentType")]
        [JsonConverter(typeof(StringEnumConverter))]
        DocumentType DocumentType { get; }

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [JsonProperty(PropertyName = "version")]
        long Version { get; set; }
    }
}
