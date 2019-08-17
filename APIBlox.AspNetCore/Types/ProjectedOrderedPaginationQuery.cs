﻿using APIBlox.AspNetCore.Attributes;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class ProjectedOrderedPaginationQuery.
    ///     Implements the <see cref="APIBlox.AspNetCore.Types.ProjectedPaginationQuery" />
    ///     Implements the <see cref="APIBlox.AspNetCore.Contracts.IOrderedQuery" />
    ///     <para>
    ///         Be sure to also call the AddFromQueryWithAlternateNamesBinder Mvc/MvcCore
    ///         builder extension method to allow alternate names to be used.
    ///     </para>
    ///     <para>
    ///         Alternates In addition to ProjectedPaginationQuery, OrderBy = $OrderBy, SortBy, $SortBy, Sort, $Sort
    ///     </para>
    /// </summary>
    /// <seealso cref="APIBlox.AspNetCore.Types.ProjectedPaginationQuery" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IOrderedQuery" />
    public class ProjectedOrderedPaginationQuery : ProjectedPaginationQuery, IOrderedQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.FilteredPaginationQuery" /> class.
        /// </summary>
        /// <inheritdoc />
        public ProjectedOrderedPaginationQuery()
        {
            Map.TryAdd("OrderBy", new[] { "$OrderBy", "SortBy", "$SortBy", "Sort", "$Sort" });
        }

        /// <summary>
        ///     Sets the order by.  Usage is determined by the API itself, please seek external documentation.
        /// </summary>
        /// <value>The order by.</value>
        /// <inheritdoc />
        [FromQueryWithAlternateNames("orderBy", "$OrderBy", "SortBy", "$SortBy", "Sort", "$Sort")]
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
