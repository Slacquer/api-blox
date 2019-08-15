using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class FilteredOrderedQuery.
    ///     Implements the <see cref="APIBlox.AspNetCore.Types.FilteredQuery" />
    ///     Implements the <see cref="APIBlox.AspNetCore.Contracts.IOrderedQuery" />
    /// </summary>
    /// <seealso cref="APIBlox.AspNetCore.Types.FilteredQuery" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IOrderedQuery" />
    public class FilteredOrderedQuery : FilteredQuery, IOrderedQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FilteredProjectedPaginationQuery" /> class.
        /// </summary>
        /// <inheritdoc />
        public FilteredOrderedQuery()
        {
            Map.TryAdd("OrderBy", new[] {"$OrderBy", "SortBy", "$SortBy", "Sort", "$Sort"});
        }

        /// <summary>
        ///     Sets the order by.  Usage is determined by the API itself, please seek external documentation.
        /// </summary>
        /// <value>The order by.</value>
        /// <inheritdoc />
        [FromQuery(Name = "orderBy")]
        public string OrderBy { get; set; }

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        /// <inheritdoc />
        protected override QueryBuilder BuildQuery()
        {
            var qb = base.BuildQuery();

            if (!OrderBy.IsEmptyNullOrWhiteSpace())
                qb.Add("orderBy", OrderBy);

            return qb;
        }
    }
}
