#region -    Using Statements    -

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#endregion

namespace APIBlox.NetCore.Types.JsonBits
{
    /// <inheritdoc />
    /// <summary>
    ///     Class CamelCaseSettings.
    ///     Implements the <see cref="Newtonsoft.Json.JsonSerializerSettings" />
    /// </summary>
    public class CamelCaseSettings : JsonSerializerSettings
    {
        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="CamelCaseSettings" /> class.
        /// </summary>
        public CamelCaseSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        #endregion
    }
}
