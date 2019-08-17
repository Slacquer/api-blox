using APIBlox.AspNetCore.Attributes;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class FilteredPaginationQuery.
    ///     Implements the <see cref="APIBlox.AspNetCore.Types.PaginationQuery" />
    ///     Implements the <see cref="APIBlox.AspNetCore.Contracts.IFilteredQuery" />
    ///     <para>
    ///         Be sure to also call the AddFromQueryWithAlternateNamesBinder Mvc/MvcCore
    ///         builder extension method to allow alternate names to be used.
    ///     </para>
    ///     <para>
    ///         Alternates In addition to PaginationQuery, Filter = $Where, Where, $Filter
    ///     </para>
    /// </summary>
    /// <seealso cref="APIBlox.AspNetCore.Types.PaginationQuery" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IFilteredQuery" />
    public class FilteredPaginationQuery : PaginationQuery, IFilteredQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.FilteredPaginationQuery" /> class.
        /// </summary>
        /// <inheritdoc />
        public FilteredPaginationQuery()
        {
            Map.TryAdd("Filter", new[] {"$Where", "Where", "$Filter"});
        }

        /// <summary>
        ///     Sets the filter (where). Usage is determined by the API itself, please seek external documentation.
        /// </summary>
        /// <value>The filter.</value>
        /// <inheritdoc />
        [FromQueryWithAlternateNames(new[] { "filter", "$Where", "Where", "$Filter" })]
        public string Filter { get; set; }

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        /// <value>The running count.</value>
        /// <inheritdoc />
        /// <inheritdoc />
        protected override QueryBuilder BuildQuery()
        {
            var qb = base.BuildQuery();

            if (!Filter.IsEmptyNullOrWhiteSpace())
                qb.Add("filter", Filter);

            return qb;
        }
    }
}
