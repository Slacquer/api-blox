using System;
using System.Collections.Generic;
using System.Text;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIBlox.NetCore
{
    internal class EventStoreDocumentEx:IEventStoreDocument
    {
        private decimal? _sortOrder;

        public virtual string Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual DocumentType DocumentType { get; set; }
        
        public string StreamId { get; set; }
        
        public long Version { get; set; }
        
        public long TimeStamp { get; set; }
        
        public decimal SortOrder
        {
            get
            {
                return (decimal) (_sortOrder = Version + GetOrderingFraction(DocumentType));
            }
        }

        public string DataType { get; set; }
        
        [JsonIgnore]
        public string Data { get; set; }


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
