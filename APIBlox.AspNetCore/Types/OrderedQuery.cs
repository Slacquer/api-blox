﻿using APIBlox.AspNetCore.Attributes;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class OrderedQuery.
    ///     <para>
    ///         Be sure to also call the AddFromQueryWithAlternateNamesBinder Mvc/MvcCore
    ///         builder extension method to allow alternate names to be used.
    ///     </para>
    ///     <para>
    ///         Alternates to OrderBy = $OrderBy, SortBy, $SortBy, Sort, $Sort
    ///     </para>
    /// </summary>
    public class OrderedQuery : Query, IOrderedQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderedQuery" /> class.
        /// </summary>
        public OrderedQuery()
        {
            Map.TryAdd("OrderBy", new[] { "$OrderBy", "SortBy", "$SortBy", "Sort", "$Sort" });
        }

        /// <summary>
        ///     Sets the order by.  Usage is determined by the API itself, please seek external documentation.
        /// </summary>
        [FromQueryWithAlternateNames("orderBy", "$OrderBy", "SortBy", "$SortBy", "Sort", "$Sort")]
        public string OrderBy { get; set; }

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        protected override QueryBuilder BuildQuery()
        {
            var qb = base.BuildQuery();

            if (!OrderBy.IsEmptyNullOrWhiteSpace())
                qb.Add("orderBy", OrderBy);

            return qb;
        }
    }
}
