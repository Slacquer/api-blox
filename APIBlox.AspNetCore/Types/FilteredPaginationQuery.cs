using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APIBlox.AspNetCore.Types
{
    /// <inheritdoc />
    /// <summary>
    ///     Class FilteredPaginationQuery.
    /// </summary>
    /// <seealso cref="T:APIBlox.AspNetCore.Types.PaginationQuery" />
    public class FilteredPaginationQuery : PaginationQuery
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.FilteredPaginationQuery" /> class.
        /// </summary>
        public FilteredPaginationQuery()
        {
        }

        internal FilteredPaginationQuery(FilteredPaginationQuery query)
            : base(query)
        {
            Map.TryAdd("Filter", new[] { "$Where", "Where", "$Filter" });
            Map.TryAdd("Select", new[] { "$Select", "Project", "$Project" });

            Select = query.Select;
            
            Filter = query.Filter;

            if (!query.FilterAlias.IsEmptyNullOrWhiteSpace())
                FilterAlias = query.FilterAlias;
            
            if (!query.SelectAlias.IsEmptyNullOrWhiteSpace())
                SelectAlias = query.SelectAlias;
        }

        /// <summary>
        ///     Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        [FromQuery(Name = "filter")]
        public string Filter { get; set; }

        [JsonProperty]
        internal string FilterAlias { get; set; } = "$filter";
        
        /// <summary>
        ///     Gets or sets the select.
        /// </summary>
        /// <value>The select.</value>
        [FromQuery(Name = "select")]
        public string Select { get; set; }

        [JsonProperty]
        internal string SelectAlias { get; set; } = "$select";


        /// <inheritdoc />
        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        protected override QueryBuilder BuildQuery()
        {
            var qb = base.BuildQuery();
            
            if (!Filter.IsEmptyNullOrWhiteSpace())
                qb.Add(FilterAlias.ToCamelCase(), Filter);

            if (!Select.IsEmptyNullOrWhiteSpace())
                qb.Add(SelectAlias.ToCamelCase(), Select);

            return qb;
        }
    }
}
