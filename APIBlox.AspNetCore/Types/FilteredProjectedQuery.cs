﻿using APIBlox.AspNetCore.Attributes;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class FilteredProjectedQuery.
    ///     <para>
    ///         Be sure to also call the AddFromQueryWithAlternateNamesBinder Mvc/MvcCore
    ///         builder extension method to allow alternate names to be used.
    ///     </para>
    ///     <para>
    ///         Alternates In addition to FilteredQuery, Select = $Select, Project, $Project
    ///     </para>
    /// </summary>
    public class FilteredProjectedQuery : FilteredQuery, IProjectedQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FilteredProjectedQuery" /> class.
        /// </summary>
        public FilteredProjectedQuery()
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
