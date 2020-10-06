using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Types.JsonBits;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace APIBlox.NetCore
{
    internal class EventSourcedJsonSerializerSettings : JsonSerializerSettings, IEventStoreJsonSerializerSettings
    {
        public EventSourcedJsonSerializerSettings(IContractResolver contractResolver = null)
        {
            ContractResolver = contractResolver ?? new PopulateNonPublicSettersContractResolver();
        }
    }

}
