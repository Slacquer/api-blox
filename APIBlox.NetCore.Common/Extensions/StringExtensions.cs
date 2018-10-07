#region -    Using Statements    -

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

#endregion

namespace APIBlox.NetCore.Extensions
{
    /// <summary>
    ///     Class StringExtensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class StringExtensions
    {
        /// <summary>
        ///     Contains with string comparison.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns><c>true</c> if the specified value contains ex; otherwise, <c>false</c>.</returns>
        public static bool ContainsEx(
            this string str, string value,
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase
        )
        {
            return str.Contains(value, comparisonType);
        }

        /// <summary>
        ///     Converts a string to another type, when null returns default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>T.</returns>
        public static T ConvertTo<T>(this string value, T defaultValue)
        {
            return value is null
                ? defaultValue
                : (T) Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        ///     EndsWith with string comparison.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool EndsWithEx(
            this string str, string value,
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase
        )
        {
            return str.EndsWith(value, comparisonType);
        }

        /// <summary>
        ///     Equals with string comparison.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool EqualsEx(
            this string str, string value,
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase
        )
        {
            return str.Equals(value, comparisonType);
        }

        /// <summary>
        ///     IndexOf with string comparison.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns>System.Int32.</returns>
        public static int IndexOfEx(
            this string str, string value,
            StringComparison comparisonType = StringComparison.Ordinal
        )
        {
            return str.IndexOf(value, comparisonType);
        }

        /// <summary>
        ///     Indicates whether a specified string is null, empty,
        ///     or when trimmed consisted only of white-space characters.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [is empty null or white space] [the specified value]; otherwise, <c>false</c>.</returns>
        public static bool IsEmptyNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim().Length == 0;
        }

        /// <summary>
        ///     LastIndexOf with string comparison.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns>System.Int32.</returns>
        public static int LastIndexOfEx(
            this string str, string value,
            StringComparison comparisonType = StringComparison.Ordinal
        )
        {
            return str.LastIndexOf(value, comparisonType);
        }

        /// <summary>
        ///     Replace with string comparison.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns>System.String.</returns>
        public static string ReplaceEx(
            this string str, string oldValue, string newValue,
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase
        )
        {
            return str.Replace(oldValue, newValue, comparisonType);
        }

        /// <summary>
        ///     StartsWith with string comparison
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool StartsWithEx(
            this string str, string value,
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase
        )
        {
            return str.StartsWith(value, comparison);
        }

        /// <summary>
        ///     Convert string to "camelCase".
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="cultureName">Name of the culture.</param>
        /// <returns>System.String.</returns>
        public static string ToCamelCase(this string str, string cultureName = "en-us")
        {
            if (str.IsEmptyNullOrWhiteSpace())
                return str;

            var bits = str.Split(new[] {" ", "_"}, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            var ci = new CultureInfo(cultureName ?? CultureInfo.CurrentCulture.DisplayName, false).TextInfo;

            for (var i = 0; i < bits.Length; i++)
                sb.Append(
                    i == 0
                        ? char.ToLowerInvariant(bits[i][0]) + bits[i].Substring(1)
                        : ci.ToTitleCase(bits[i])
                );

            return sb.ToString();
        }

        /// <summary>
        ///     Convert string to "PascalCase".
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="cultureName">Name of the culture.</param>
        /// <returns>System.String.</returns>
        public static string ToPascalCase(this string str, string cultureName = "en-us")
        {
            if (str.IsEmptyNullOrWhiteSpace())
                return null;

            var tmp = str.ToCamelCase(cultureName);

            return char.ToUpperInvariant(tmp[0]) + tmp.Substring(1);
        }
    }
}
