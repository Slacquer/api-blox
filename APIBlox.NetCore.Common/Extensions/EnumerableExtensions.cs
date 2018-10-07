#region -    Using Statements    -

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion

namespace APIBlox.NetCore.Extensions
{
    /// <summary>
    ///     Class EnumerableExtensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     Flattens the specified enumerable, like <see cref="System.Linq.Enumerable" />.
        ///     SelectMany, but traverses all levels of the same object.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>IEnumerable{TSource}.</returns>
        public static IEnumerable<TSource> Flatten<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> enumerable,
            Func<TSource, bool> filter = null
        )
        {
            var lst = source.ToList();

            var flattened = !(filter is null)
                ? lst.Where(filter)
                : lst;

            return lst.Aggregate(flattened, (c, elm) => c.Concat(enumerable(elm).Flatten(enumerable, filter)));
        }

        /// <summary>
        ///     Adds an item to collection  if it doesn't already exist.
        /// </summary>
        /// <typeparam name="TCollection">The type of the t collection.</typeparam>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="col">The col.</param>
        /// <param name="value">The value.</param>
        /// <returns>TCollection.</returns>
        public static TCollection TryAdd<TCollection, TType>(this TCollection col, TType value)
            where TCollection : ICollection<TType>
        {
            if (col.All(t => t.GetType() != value.GetType()))
                col.Add(value);

            return col;
        }
    }
}
