﻿using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class FilteredQuery.
    /// </summary>
    /// <seealso cref="APIBlox.AspNetCore.Types.OrderedQuery" />
    public class FilteredQuery : OrderedQuery
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.FilteredQuery" /> class.
        /// </summary>
        public FilteredQuery()
        {
            Map.TryAdd("Filter", new[] {"$Where", "Where", "$Filter"});
            Map.TryAdd("Select", new[] {"$Select", "Project", "$Project"});
        }

        /// <summary>
        ///     Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        [FromQuery(Name = "filter")]
        public string Filter { get; set; }

        /// <summary>
        ///     Gets or sets the select.
        /// </summary>
        /// <value>The select.</value>
        [FromQuery(Name = "select")]
        public string Select { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        protected override QueryBuilder BuildQuery()
        {
            var qb = base.BuildQuery();

            if (!Filter.IsEmptyNullOrWhiteSpace())
                qb.Add("filter", Filter);

            if (!Select.IsEmptyNullOrWhiteSpace())
                qb.Add("select", Select);

            return qb;
        }
    }
}