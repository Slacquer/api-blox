using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class FilteredProjectedPaginationQuery.
    ///     Implements the <see cref="APIBlox.AspNetCore.Types.FilteredPaginationQuery" />
    ///     Implements the <see cref="APIBlox.AspNetCore.Contracts.IProjectedQuery" />
    /// </summary>
    /// <seealso cref="APIBlox.AspNetCore.Types.FilteredPaginationQuery" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IProjectedQuery" />
    public class FilteredProjectedPaginationQuery : FilteredPaginationQuery, IProjectedQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FilteredProjectedPaginationQuery" /> class.
        /// </summary>
        /// <inheritdoc />
        public FilteredProjectedPaginationQuery()
        {
            Map.TryAdd("Select", new[] {"$Select", "Project", "$Project"});
        }

        /// <summary>
        ///     Sets the select (projection).  Usage is determined by the API itself, please seek external documentation.
        /// </summary>
        /// <value>The select.</value>
        /// <inheritdoc />
        [FromQuery(Name = "select")]
        public string Select { get; set; }

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        /// <inheritdoc />
        protected override QueryBuilder BuildQuery()
        {
            var qb = base.BuildQuery();

            if (!Select.IsEmptyNullOrWhiteSpace())
                qb.Add("select", Select);

            return qb;
        }
    }
}
