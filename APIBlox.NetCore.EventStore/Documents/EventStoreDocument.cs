using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIBlox.NetCore.Documents
{
    /// <summary>
    ///     Class EventStoreDocument.
    /// </summary>
    public class EventStoreDocument
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [JsonProperty(PropertyName = "id")]
        public virtual string Id { get; set; }

        /// <summary>
        ///     Gets or sets the type of the document.
        /// </summary>
        /// <value>The type of the document.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual DocumentType DocumentType { get; set; }

        /// <summary>
        ///     Gets or sets the stream identifier.
        /// </summary>
        /// <value>The stream identifier.</value>
        public string StreamId { get; set; }

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public long Version { get; set; }

        /// <summary>
        ///     Gets or sets the time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        public long TimeStamp { get; set; }

        /// <summary>
        ///     Gets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public decimal SortOrder => Version + GetOrderingFraction(DocumentType);

        /// <summary>
        ///     Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public string DataType { get; set; }

        /// <summary>
        ///     Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public object Data { get; set; }

        private static decimal GetOrderingFraction(DocumentType documentType)
        {
            return documentType switch
            {
                DocumentType.Root => 0.3M,
                DocumentType.Snapshot => 0.2M,
                DocumentType.Event => 0.1M,
                _ => throw new NotSupportedException($"Document type '{documentType}' is not supported.")
            };
        }
    }
}
