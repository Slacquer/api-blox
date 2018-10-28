using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace APIBlox.AspNetCore.Types
{
    /// <inheritdoc />
    /// <summary>
    ///     Class FilteredPaginationQuery.
    /// </summary>
    /// <seealso cref="T:APIBlox.AspNetCore.Types.PaginationQuery" />
    public class FilteredPaginationQuery : PaginationQuery
    {
        private const string FilterParam = "$filter";
        private const string OrderByParam = "$orderby";
        private const string SelectParam = "$select";

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
            PaginationMap.TryAdd("OrderBy", new[] {"$OrderBy"});
            PaginationMap.TryAdd("Filter", new[] {"$Where", "Where", "$Filter"});
            PaginationMap.TryAdd("Select", new[] {"$Select", "Project", "$Project"});

            Select = query.Select;
            OrderBy = query.OrderBy;
            Filter = query.Filter;
        }

        /// <summary>
        ///     Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter { get; set; }

        /// <summary>
        ///     Gets or sets the order by.
        /// </summary>
        /// <value>The order by.</value>
        public string OrderBy { get; set; }

        /// <summary>
        ///     Gets or sets the select.
        /// </summary>
        /// <value>The select.</value>
        public string Select { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        protected override QueryBuilder BuildQuery()
        {
            var qb = base.BuildQuery();

            if (!OrderBy.IsEmptyNullOrWhiteSpace())
                qb.Add(OrderByParam, OrderBy);

            if (!Filter.IsEmptyNullOrWhiteSpace())
                qb.Add(FilterParam, Filter);

            if (!Select.IsEmptyNullOrWhiteSpace())
                qb.Add(SelectParam, Select);

            return qb;
        }
    }
}
