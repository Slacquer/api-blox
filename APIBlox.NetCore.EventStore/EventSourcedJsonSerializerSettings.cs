using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Types.JsonBits;
using Newtonsoft.Json;

namespace APIBlox.NetCore
{
    internal class EventSourcedJsonSerializerSettings : IEventStoreJsonSerializerSettings
    {
        public EventSourcedJsonSerializerSettings(JsonSerializerSettings settings)
        {
            Settings = settings ?? new CamelCaseSettings
            {
                ContractResolver = new CamelCasePopulateNonPublicSettersContractResolver()
            };
        }
        public JsonSerializerSettings Settings { get; }
    }
}
