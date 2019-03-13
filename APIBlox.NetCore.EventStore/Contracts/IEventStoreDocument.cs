using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIBlox.NetCore.Contracts
{
    public interface IEventStoreDocument
    {
        [JsonProperty(PropertyName = "id")]
        string Id { get; }

        [JsonProperty(PropertyName = "streamId")]
        string StreamId { get; }
        
        [JsonProperty(PropertyName = "sortOrder")]
        decimal SortOrder { get; }

        [JsonProperty(PropertyName = "documentType")]
        [JsonConverter(typeof(StringEnumConverter))]
        DocumentType DocumentType { get; }

        [JsonProperty(PropertyName = "version")]
        long Version { get; set; }
    }
}
