using APIBlox.AspNetCore.Attributes;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class ProjectedPaginationQuery.
    ///     Implements the <see cref="APIBlox.AspNetCore.Types.PaginationQuery" />
    ///     Implements the <see cref="APIBlox.AspNetCore.Contracts.IProjectedQuery" />
    ///     <para>
    ///         Be sure to also call the AddFromQueryWithAlternateNamesBinder Mvc/MvcCore
    ///         builder extension method to allow alternate names to be used.
    ///     </para>
    ///     <para>
    ///         Alternates In addition to ProjectedQuery, Select = $Select, Project, $Project
    ///     </para>
    /// </summary>
    /// <seealso cref="APIBlox.AspNetCore.Types.PaginationQuery" />
    /// <seealso cref="APIBlox.AspNetCore.Contracts.IProjectedQuery" />
    public class ProjectedPaginationQuery : PaginationQuery, IProjectedQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectedPaginationQuery" /> class.
        /// </summary>
        /// <inheritdoc />
        public ProjectedPaginationQuery()
        {
            Map.TryAdd("Select", new[] {"$Select", "Project", "$Project"});
        }

        /// <summary>
        ///     Sets the select (projection).  Usage is determined by the API itself, please seek external documentation.
        /// </summary>
        /// <value>The select.</value>
        /// <inheritdoc />
        [FromQuery(Name = "select")]
        [FromQueryWithAlternateNames(new[] { "select", "$Select", "Project", "$Project" })]
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
