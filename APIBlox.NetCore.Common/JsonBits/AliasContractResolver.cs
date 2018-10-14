using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using APIBlox.NetCore.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace APIBlox.NetCore.JsonBits
{
    /// <inheritdoc />
    /// <summary>
    ///     Class AliasContractResolver.
    /// <para>
    ///     Given a dictionary of aliases this resolver will map either to
    ///     properties when Deserializing, or to strings when serializing.
    /// </para>
    /// </summary>
    /// <seealso cref="T:Newtonsoft.Json.Serialization.DefaultContractResolver" />
    public class AliasContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<string, string[]> _map;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.NetCore.JsonBits.AliasContractResolver" /> class.
        /// </summary>
        /// <param name="propertyAliasMap">The property alias map.</param>
        public AliasContractResolver(Dictionary<string, string[]> propertyAliasMap)
        {
            _map = propertyAliasMap;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given
        ///     <see cref="T:System.Reflection.MemberInfo" /> using the provided map.
        /// </summary>
        /// <param name="member">The member to create a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for.</param>
        /// <param name="memberSerialization">The member's parent <see cref="T:Newtonsoft.Json.MemberSerialization" />.</param>
        /// <returns>
        ///     A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given
        ///     <see cref="T:System.Reflection.MemberInfo" />.
        /// </returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            var propName = _map.FirstOrDefault(kvp =>
                kvp.Value.Any(s => s.EqualsEx(member.Name))
            );

            if (!(propName.Value is null))
                prop.PropertyName = propName.Key;

            return prop;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Resolves the name of the property using the provided map.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Resolved name of the property.</returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            var propName = _map.FirstOrDefault(kvp =>
                kvp.Key.EqualsEx(propertyName)
                || kvp.Value.Any(s => s.EqualsEx(propertyName))
            );

            return !(propName.Value is null) ? propName.Key : base.ResolvePropertyName(propertyName);
        }
    }
}
