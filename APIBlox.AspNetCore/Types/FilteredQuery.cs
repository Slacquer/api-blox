using APIBlox.AspNetCore.Attributes;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class FilteredQuery.
    ///     <para>
    ///         Be sure to also call the AddFromQueryWithAlternateNamesBinder Mvc/MvcCore
    ///         builder extension method to allow alternate names to be used.
    ///     </para>
    ///     <para>
    ///         Alternates to Filter = $Where, Where, $Filter
    ///     </para>
    /// </summary>
    public class FilteredQuery : Query, IFilteredQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.FilteredQuery" /> class.
        /// </summary>
        public FilteredQuery()
        {
            Map.TryAdd("Filter", new[] { "$Where", "Where", "$Filter" });
        }

        /// <summary>
        ///     Sets the filter (where). Usage is determined by the API itself, please seek external documentation.
        /// </summary>
        [FromQueryWithAlternateNames("filter", "$Where", "$WHERE", "$whERE", "$Where", "Where", "$Filter")]
        public string Filter { get; set; }

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
