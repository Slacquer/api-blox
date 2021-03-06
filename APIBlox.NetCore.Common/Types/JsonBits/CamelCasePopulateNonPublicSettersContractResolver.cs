﻿using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace APIBlox.NetCore.Types.JsonBits
{
    /// <summary>
    ///     Class CamelCasePopulateNonPublicSettersContractResolver.
    ///     <para>
    ///         Use me when you need your serializable objects private setters to be writable.
    ///     </para>
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.Serialization.DefaultContractResolver" />
    /// <inheritdoc cref="DefaultContractResolver" />
    /// <seealso cref="T:Newtonsoft.Json.Serialization.DefaultContractResolver" />
    [DebuggerStepThrough]
    public class CamelCasePopulateNonPublicSettersContractResolver : CamelCasePropertyNamesContractResolver
    {
        /// <inheritdoc />
        /// <summary>
        ///     Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given
        ///     <see cref="T:System.Reflection.MemberInfo" />.
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

            if (prop.Writable)
                return prop;

            var property = member as PropertyInfo;

            if (property == null)
                return prop;

            prop.Writable = property.GetSetMethod(true) != null;

            return prop;
        }
    }
}
