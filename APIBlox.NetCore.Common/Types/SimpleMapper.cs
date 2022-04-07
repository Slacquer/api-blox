using APIBlox.NetCore.Extensions;
using Newtonsoft.Json;

namespace APIBlox.NetCore.Types
{
    /// <summary>
    ///     Class SimpleMapper
    /// </summary>
    public static class SimpleMapper
    {
        /// <summary>
        ///     Maps one object to another with matching properties.
        /// </summary>
        /// <typeparam name="TDest">The type of the t destination.</typeparam>
        /// <param name="src">The source.</param>
        /// <param name="dest">The TDest instance, if null a new one will be created.</param>
        /// <param name="settings">
        ///     JsonSerializerSettings, when null, then...
        ///     <para>
        ///         settings = settings ?? new JsonSerializerSettings
        ///         {
        ///         ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        ///         PreserveReferencesHandling = PreserveReferencesHandling.All
        ///         };
        ///     </para>
        /// </param>
        /// <returns>TDest.</returns>
        public static TDest MapTo<TDest>(this object src, TDest dest = default,
            JsonSerializerSettings settings = null
        )
            where TDest : new()
        {
            settings ??= new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };

            var ret = dest == null ? new TDest() : dest;

            if (src is string jsonString && jsonString.IsJson())
                JsonConvert.PopulateObject(jsonString, ret, settings);
            else
                JsonConvert.PopulateObject(JsonConvert.SerializeObject(src, settings), ret, settings);

            return ret;
        }
    }
}
