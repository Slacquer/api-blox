using APIBlox.AspNetCore.Attributes;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class ProjectedQuery.
    ///     <para>
    ///         Be sure to also call the AddFromQueryWithAlternateNamesBinder Mvc/MvcCore
    ///         builder extension method to allow alternate names to be used.
    ///     </para>
    ///     <para>
    ///         Alternates to Select = $Select, Project, $Project
    ///     </para>
    /// </summary>
    public class ProjectedQuery : Query, IProjectedQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.ProjectedQuery" /> class.
        /// </summary>
        public ProjectedQuery()
        {
            Map.TryAdd("Select", new[] { "$Select", "Project", "$Project" });
        }

        /// <summary>
        ///     Sets the select (projection).  Usage is determined by the API itself, please seek external documentation.
        /// </summary>
        [FromQueryWithAlternateNames("select", "$Select", "Project", "$Project")]
        public string Select { get; set; }

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
