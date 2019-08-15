using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIBlox.NetCore.Extensions
{
    /// <summary>
    ///     Class StringExtensions.
    /// </summary>

    //[DebuggerStepThrough]
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
        ///     Reverses the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.String.</returns>
        public static string Reverse(this string str)
        {
            return str.IsEmptyNullOrWhiteSpace() ? null : string.Join("", Enumerable.Reverse(str));
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
        ///     Removes the trailing whack \ or / from a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string RemoveTrailingWhack(this string value)
        {
            if (value is null)
                return null;

            var trimmed = value.Trim();

            return trimmed.EndsWith(@"\") || trimmed.EndsWith(@"/") ? trimmed.Substring(0, trimmed.Length - 1) : trimmed;
        }

        /// <summary>
        ///     Add a trailing whack \ or / from a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string AddTrailingWhack(this string value)
        {
            if (value is null)
                return null;

            var trimmed = value.Trim();
            var slash = trimmed.Contains(@"\") ? @"\" : "/";

            return trimmed.EndsWith(@"\") || trimmed.EndsWith(@"/") ? trimmed : $"{trimmed}{slash}";
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
        /// <param name="trimmed">Trim first before comparing.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool EqualsEx(
            this string str, string value,
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase,
            bool trimmed = true
        )
        {
            if (str is null || value is null)
                return false;

            return trimmed
                ? str.Trim().Equals(value.Trim(), comparisonType)
                : str.Equals(value, comparisonType);
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
        ///     Determines whether the specified string value is json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is json; otherwise, <c>false</c>.</returns>
        public static bool IsJson(this string value)
        {
            try
            {
                JToken.Parse(value);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
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
                sb.Append(i == 0
                    ? char.ToLowerInvariant(bits[i][0]) + bits[i].Substring(1)
                    : ci.ToTitleCase(bits[i])
                );

            return sb.ToString();
        }

        /// <summary>
        ///     Attempts to convert a string into a phrase.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="onlyFirstCapitalized">Convert all words (except the first) to lowerCase.</param>
        /// <returns>System.String.</returns>
        public static string ToPhrase(this string str, bool onlyFirstCapitalized = true)
        {
            if (str.IsEmptyNullOrWhiteSpace())
                return str;

            var result = Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );

            if (!onlyFirstCapitalized)
                return result;

            result = result.ToLower();

            return $"{result.Substring(0, 1).ToUpper()}{result.Substring(1)}";
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
