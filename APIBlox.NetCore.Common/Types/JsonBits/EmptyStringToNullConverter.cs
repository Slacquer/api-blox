using System;
using APIBlox.NetCore.Extensions;
using Newtonsoft.Json;

namespace APIBlox.NetCore.Types.JsonBits
{
    /// <inheritdoc />
    /// <summary>
    ///     Class EmptyStringToNullConverter.
    /// </summary>
    /// <seealso cref="T:Newtonsoft.Json.JsonConverter" />
    public class EmptyStringToNullConverter : JsonConverter
    {
        /// <inheritdoc />
        /// <summary>
        ///     Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
        /// </summary>

        public override bool CanWrite => false;

        /// <inheritdoc />
        /// <summary>
        ///     Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is null)
                return null;

            var text = reader.Value.ToString();

            return text.IsEmptyNullOrWhiteSpace() ? null : text;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}
