using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Marker Interface
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.Serialization.IContractResolver" />
    public interface IJsonBitsContractResolver : IContractResolver
    {
    }
}
