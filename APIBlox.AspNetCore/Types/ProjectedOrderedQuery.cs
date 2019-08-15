using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class ProjectedOrderedQuery.
    ///     Implements the <see cref="APIBlox.AspNetCore.Types.ProjectedQuery" />
    ///     Implements the <see cref="APIBlox.AspNetCore.Contracts.IOrderedQuery" />
    /// </summary>
    /// <seealso cref="APIBlox.AspNetCore.Types.ProjectedQuery" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IOrderedQuery" />
    public class ProjectedOrderedQuery : ProjectedQuery, IOrderedQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectedOrderedQuery" /> class.
        /// </summary>
        /// <inheritdoc />
        public ProjectedOrderedQuery()
        {
            Map.TryAdd("OrderBy", new[] {"$OrderBy", "SortBy", "$SortBy", "Sort", "$Sort"});
        }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the order by.  Usage is determined by the API itself, please seek external documentation.
        /// </summary>
        /// <value>The order by.</value>
        /// <summary>
        ///     Gets or sets the order by.
        /// </summary>
        /// <value>The order by.</value>
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
