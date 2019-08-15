using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class FilteredQuery.
    ///     Implements the <see cref="APIBlox.AspNetCore.Types.Query" />
    ///     Implements the <see cref="APIBlox.AspNetCore.Contracts.IFilteredQuery" />
    /// </summary>
    /// <seealso cref="APIBlox.AspNetCore.Types.Query" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IFilteredQuery" />
    public class FilteredQuery : Query, IFilteredQuery
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.FilteredQuery" /> class.
        /// </summary>
        public FilteredQuery()
        {
            Map.TryAdd("Filter", new[] {"$Where", "Where", "$Filter"});
        }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the filter (where). Usage is determined by the API itself, please seek external documentation.
        /// </summary>
        /// <value>The filter.</value>
        [FromQuery(Name = "filter")]
        public string Filter { get; set; }

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

            return qb;
        }
    }
}
