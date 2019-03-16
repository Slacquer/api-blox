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
        ///     The separator
        /// </summary>
        protected const char Separator = '-';

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [JsonProperty(PropertyName = "id")]
        public virtual string Id { get; set; }

        /// <summary>
        ///     Gets or sets the type of the metadata.
        /// </summary>
        /// <value>The type of the metadata.</value>
        //[JsonProperty(PropertyName = "metadataType")]
        public string MetadataType { get; set; }

        /// <summary>
        ///     Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        //[JsonProperty(PropertyName = "metadata")]
        public object Metadata { get; set; }

        /// <summary>
        ///     Gets or sets the type of the document.
        /// </summary>
        /// <value>The type of the document.</value>
        //[JsonProperty(PropertyName = "documentType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual DocumentType DocumentType { get; set; }

        /// <summary>
        ///     Gets or sets the stream identifier.
        /// </summary>
        /// <value>The stream identifier.</value>
        //[JsonProperty(PropertyName = "streamId")]
        public string StreamId { get; set; }

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        //[JsonProperty(PropertyName = "version")]
        public long Version { get; set; }

        /// <summary>
        ///     Gets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        //[JsonProperty(PropertyName = "sortOrder")]
        public decimal SortOrder => Version + GetOrderingFraction(DocumentType);

        /// <summary>
        ///     Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        //[JsonProperty(PropertyName = "dataType")]
        public string DataType { get; set; }

        /// <summary>
        ///     Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        //[JsonProperty(PropertyName = "data")]
        public object Data { get; set; }


        private static decimal GetOrderingFraction(DocumentType documentType)
        {
            switch (documentType)
            {
                case DocumentType.Root:
                    return 0.3M;
                case DocumentType.Snapshot:
                    return 0.2M;
                case DocumentType.Event:
                    return 0.1M;
                default:
                    throw new NotSupportedException($"Document type '{documentType}' is not supported.");
            }
        }
    }
}
