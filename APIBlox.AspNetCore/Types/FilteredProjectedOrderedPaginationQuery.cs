using APIBlox.AspNetCore.Attributes;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class FilteredProjectedOrderedPaginationQuery.
    ///     Implements the <see cref="APIBlox.AspNetCore.Types.FilteredProjectedPaginationQuery" />
    ///     Implements the <see cref="APIBlox.AspNetCore.Contracts.IOrderedQuery" />
    ///     <para>
    ///         Be sure to also call the AddFromQueryWithAlternateNamesBinder Mvc/MvcCore
    ///         builder extension method to allow alternate names to be used.
    ///     </para>
    ///     <para>
    ///         Alternates In addition to FilteredProjectedPaginationQuery, OrderBy = $OrderBy, SortBy, $SortBy, Sort, $Sort
    ///     </para>
    /// </summary>
    /// <seealso cref="APIBlox.AspNetCore.Types.FilteredProjectedPaginationQuery" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IOrderedQuery" />
    public class FilteredProjectedOrderedPaginationQuery : FilteredProjectedPaginationQuery, IOrderedQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderedQuery" /> class.
        /// </summary>
        /// <inheritdoc />
        public FilteredProjectedOrderedPaginationQuery()
        {
            Map.TryAdd("OrderBy", new[] { "$OrderBy", "SortBy", "$SortBy", "Sort", "$Sort" });
        }

        /// <summary>
        ///     Sets the order by.  Usage is determined by the API itself, please seek external documentation.
        /// </summary>
        /// <value>The order by.</value>
        /// <inheritdoc />
        [FromQuery(Name = "orderBy")]
        [FromQueryWithAlternateNames("orderBy", "$OrderBy", "SortBy", "$SortBy", "Sort", "$Sort" )]
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
