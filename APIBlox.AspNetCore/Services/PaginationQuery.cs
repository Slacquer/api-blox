using System.Collections.Generic;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.JsonBits;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIBlox.AspNetCore.Services
{
    /// <summary>
    ///     Class PaginationQuery.
    /// </summary>
    public class PaginationQuery
    {
        private const string CountParam = "$count";
        private const string FilterParam = "$filter";
        private const string OrderByParam = "$orderby";
        private const string SelectParam = "$select";
        private const string SkipParam = "$skip";
        private const string TopParam = "$top";

        private static readonly Dictionary<string, string[]> InMap = new Dictionary<string, string[]>
        {
            {"Skip", new[] {"$Skip", "Offset", "$Offset"}},
            {"Top", new[] {"$Top", "Limit", "$Limit", "Take", "$Take"}},
            {"OrderBy", new[] {"$OrderBy"}},
            {"Filter", new[] {"$Where", "Where", "$Filter"}},
            {"Select", new[] {"$Select", "Project", "$Project"}}
        };

        internal static readonly JsonSerializerSettings AliasesInSettings = new JsonSerializerSettings
        {
            ContractResolver = new AliasContractResolver(InMap)
        };

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaginationQuery" /> class.
        /// </summary>
        public PaginationQuery()
        {
        }

        internal PaginationQuery(PaginationQuery query)
        {
            Skip = query.Skip;
            Top = query.Top;
            OrderBy = query.OrderBy;
            Filter = query.Filter;
            Select = query.Select;
            Undefined = query.Undefined;
        }

        internal int? Count { get; set; }

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
        ///     Gets or sets the undefined parameters.
        /// </summary>
        /// <value>The other.</value>
        [JsonExtensionData]
        public IDictionary<string, JToken> Undefined { get; set; }

        /// <summary>
        ///     Gets or sets the select.
        /// </summary>
        /// <value>The select.</value>
        public string Select { get; set; }

        /// <summary>
        ///     Gets or sets the skip.
        /// </summary>
        /// <value>The skip.</value>
        public int? Skip { get; set; }

        /// <summary>
        ///     Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public int? Top { get; set; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance if the form of a query string.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            var qb = new QueryBuilder();

            if (Skip.HasValue)
                qb.Add(SkipParam, Skip.Value.ToString());

            if (Top.HasValue)
                qb.Add(TopParam, Top.Value.ToString());

            if (Count.HasValue)
                qb.Add(CountParam, Count.Value.ToString());

            if (!OrderBy.IsEmptyNullOrWhiteSpace())
                qb.Add(OrderByParam, OrderBy);

            if (!Filter.IsEmptyNullOrWhiteSpace())
                qb.Add(FilterParam, Filter);

            if (!Select.IsEmptyNullOrWhiteSpace())
                qb.Add(SelectParam, Select);

            if (Undefined is null)
                return qb.ToString();

            foreach (var p in Undefined)
                qb.Add(p.Key, p.Value.ToString());

            return qb.ToString();
        }
    }
}
