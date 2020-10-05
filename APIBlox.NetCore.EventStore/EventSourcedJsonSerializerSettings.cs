using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Types.JsonBits;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace APIBlox.NetCore
{
    internal class EventSourcedJsonSerializerSettings : IEventStoreJsonSerializerSettings
    {
        public EventSourcedJsonSerializerSettings(JsonSerializerSettings settings)
        {
            //Settings = settings ?? new CamelCaseSettings
            //{
            //    ContractResolver = new CamelCasePopulateNonPublicSettersContractResolver()
            //};
            Settings = settings;
        }
        public JsonSerializerSettings Settings { get; }
    }

}
