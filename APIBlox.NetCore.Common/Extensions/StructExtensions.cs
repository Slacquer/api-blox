namespace APIBlox.NetCore.Extensions
{
    /// <summary>
    ///     Class NumberExtensions.
    /// </summary>
    public static class StructExtensions
    {
        /// <summary>
        ///     Determines whether [is null or default] [the specified value].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [is null or zero] [the specified value]; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrDefault<T>(this T? value)
            where T : struct
        {
            return !value.HasValue || Equals(value, default(T));
        }
    }
}
