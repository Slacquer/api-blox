using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <inheritdoc />
    /// <summary>
    ///     Class OrderedQuery.
    /// </summary>
    public class OrderedQuery : Query, IOrderedQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderedQuery" /> class.
        /// </summary>
        public OrderedQuery()
        {
            Map.TryAdd("OrderBy", new[] {"$OrderBy", "SortBy", "$SortBy", "Sort", "$Sort"});
        }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the order by.  Usage is determined by the API itself.
        /// </summary>
        /// <value>The order by.</value>
        [FromQuery(Name = "orderBy")]
        public string OrderBy { get; set; }

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        protected override QueryBuilder BuildQuery()
        {
            var qb = new QueryBuilder();

            if (!OrderBy.IsEmptyNullOrWhiteSpace())
                qb.Add("orderBy", OrderBy);

            if (Undefined is null)
                return qb;

            foreach (var p in Undefined)
                qb.Add(p.Key.ToCamelCase(), p.Value);

            return qb;
        }
    }
}
