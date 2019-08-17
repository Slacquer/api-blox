using APIBlox.AspNetCore.Attributes;
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
    ///     <para>
    ///         Be sure to also call the AddFromQueryWithAlternateNamesBinder Mvc/MvcCore
    ///         builder extension method to allow alternate names to be used.
    ///     </para>
    ///     <para>
    ///         Alternates to Filter = $Where, Where, $Filter
    ///     </para>
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
        [FromQueryWithAlternateNames(new[] { "filter", "$Where", "Where", "$Filter" })]
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
