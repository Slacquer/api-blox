using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <inheritdoc cref="Query" />
    /// <summary>
    ///     Class ProjectedQuery.
    /// </summary>
    /// <seealso cref="T:APIBlox.AspNetCore.Types.OrderedQuery" />
    public class ProjectedQuery : Query, IProjectedQuery
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.ProjectedQuery" /> class.
        /// </summary>
        public ProjectedQuery()
        {
            Map.TryAdd("Select", new[] {"$Select", "Project", "$Project"});
        }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the select (projection).  Usage is determined by the API itself.
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

            if (!Select.IsEmptyNullOrWhiteSpace())
                qb.Add("select", Select);

            return qb;
        }
    }
}
