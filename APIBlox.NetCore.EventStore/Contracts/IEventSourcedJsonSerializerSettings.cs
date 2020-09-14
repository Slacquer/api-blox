using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Marker Interface
    /// </summary>
    public interface IEventSourcedJsonSerializerSettings
    {
        /// <summary>
        ///     Gets the settings.
        /// </summary>
        JsonSerializerSettings Settings { get; }
    }

    internal class EventSourcedJsonSerializerSettings : IEventSourcedJsonSerializerSettings
    {
        public EventSourcedJsonSerializerSettings(JsonSerializerSettings settings)
        {
            Settings = settings;
        }
        public JsonSerializerSettings Settings { get; }
    }
}
