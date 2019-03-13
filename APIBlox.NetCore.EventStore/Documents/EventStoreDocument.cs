using System;
using System.Linq;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Exceptions;
using APIBlox.NetCore.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace APIBlox.NetCore.Documents
{
    public class EventStoreDocument : IEventStoreDocument
    {
        protected const char Separator = '-';

        [JsonProperty(PropertyName = "id")]
        public virtual string Id { get; set; }
        
        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }

        [JsonProperty(PropertyName = "_ts")]
        public long TimeStamp { get; set; }

        [JsonProperty(PropertyName = "metadataType")]
        public string MetadataType { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public object Metadata { get; set; }

        [JsonProperty(PropertyName = "documentType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual DocumentType DocumentType { get; set; }

        [JsonProperty(PropertyName = "streamId")]
        public string StreamId { get; set; }

        [JsonProperty(PropertyName = "version")]
        public long Version { get; set; }

        [JsonProperty(PropertyName = "sortOrder")]
        public decimal SortOrder => Version + GetOrderingFraction(DocumentType);

        public static EventStoreDocument Parse(string jsonDocument, JsonSerializerSettings jsonSerializerSettings)
        {
            if (jsonDocument == null)
                throw new ArgumentNullException(nameof(jsonDocument));

            if (jsonSerializerSettings == null)
                throw new ArgumentNullException(nameof(jsonSerializerSettings));

            var documentType = FindDocumentType(jsonDocument);

            if (!documentType.HasValue)
                throw new DocumentMalformedException($"jsonDocument does not appear to have an {nameof(DocumentType)} value!");

            EventStoreDocument ret;

            switch (documentType)
            {
                case DocumentType.Root:
                    ret = JsonConvert.DeserializeObject<RootDocument>(jsonDocument, jsonSerializerSettings);
                    break;

                case DocumentType.Snapshot:

                    var ss = JsonConvert.DeserializeObject<SnapshotDocument>(jsonDocument, jsonSerializerSettings);
                    ss.SnapshotData = JsonConvert.DeserializeObject(ss.SnapshotData.ToString(), Type.GetType(ss.SnapshotType), jsonSerializerSettings);
                    
                    ret = ss;

                    break;

                case DocumentType.Event:
                    var ev = JsonConvert.DeserializeObject<EventDocument>(jsonDocument, jsonSerializerSettings);
                    ev.EventData = JsonConvert.DeserializeObject(ev.EventData.ToString(), Type.GetType(ev.EventType), jsonSerializerSettings);
                    ret = ev;
                    break;

                default:
                    throw new NotSupportedException(
                        $"Cannot parse document of type '{jsonDocument.GetType().AssemblyQualifiedName}' with DocumentType '{jsonDocument}'."
                    );
            }

            if (!(ret.Metadata is null))
                ret.Metadata = JsonConvert.DeserializeObject(ret.Metadata.ToString(), Type.GetType(ret.MetadataType), jsonSerializerSettings);

            return ret;
        }

        private static DocumentType? FindDocumentType(string result)
        {
            var jo = JObject.Parse(result);
            var type = jo.DescendantsAndSelf().OfType<JProperty>().FirstOrDefault(t => t.Name.EqualsEx("documentType"));
            
            return type is null 
                ? (DocumentType?) null 
                : (DocumentType)Enum.Parse(typeof(DocumentType), type.Value.ToString());
        }

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
