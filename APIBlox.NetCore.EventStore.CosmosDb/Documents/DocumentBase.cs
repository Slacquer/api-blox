using System;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIBlox.NetCore.Documents
{


    public abstract class DocumentBase:IEventStoreDocument
    {
        protected const char Separator = '-';

        public abstract string Id { get; }

        public string PartitionBy { get; set; }

        [JsonProperty(PropertyName = "_etag")] public string ETag { get; set; }

        [JsonProperty(PropertyName = "_ts")] 
        public long TimeStamp { get; set; }

        public string MetadataType { get; set; }

        public object Metadata { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public abstract DocumentType DocumentType { get; }

        public string StreamId { get; set; }

        public long Version { get; set; }

        public decimal SortOrder => Version + GetOrderingFraction(DocumentType);

        public static DocumentBase Parse(Document document, Func<object> initializeSnapshotObject, JsonSerializerSettings jsonSerializerSettings)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (jsonSerializerSettings == null)
                throw new ArgumentNullException(nameof(jsonSerializerSettings));

            var documentType = document.GetPropertyValue<DocumentType>(nameof(DocumentType).ToCamelCase());
            documentType = documentType == 0 ? document.GetPropertyValue<DocumentType>(nameof(DocumentType)) : documentType;

            DocumentBase ret;

            switch (documentType)
            {
                case DocumentType.Root:
                    ret = JsonConvert.DeserializeObject<RootDocument>(document.ToString(), jsonSerializerSettings);
                    break;

                case DocumentType.Snapshot:

                    var ss = JsonConvert.DeserializeObject<SnapshotDocument>(document.ToString(), jsonSerializerSettings);

                    if (initializeSnapshotObject is null)
                    {
                        ss.SnapshotData =
                            JsonConvert.DeserializeObject(ss.SnapshotData.ToString(), Type.GetType(ss.SnapshotType), jsonSerializerSettings);
                    }
                    else
                    {
                        var obj = initializeSnapshotObject();

                        JsonConvert.PopulateObject(ss.SnapshotData.ToString(), obj, jsonSerializerSettings);

                        ss.SnapshotData = obj;
                    }

                    ret = ss;

                    break;

                case DocumentType.Event:
                    var ev = JsonConvert.DeserializeObject<EventDocument>(document.ToString(), jsonSerializerSettings);
                    ev.EventData = JsonConvert.DeserializeObject(ev.EventData.ToString(), Type.GetType(ev.EventType), jsonSerializerSettings);
                    ret = ev;
                    break;

                default:
                    throw new NotSupportedException(
                        $"Cannot parse document of type '{document.GetType().AssemblyQualifiedName}' with DocumentType '{document}'."
                    );
            }

            if (!(ret.Metadata is null))
                ret.Metadata = JsonConvert.DeserializeObject(ret.Metadata.ToString(), Type.GetType(ret.MetadataType), jsonSerializerSettings);

            return ret;
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
