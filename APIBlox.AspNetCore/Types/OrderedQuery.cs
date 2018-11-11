using System.Collections.Generic;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class OrderedQuery.
    /// </summary>
    public class OrderedQuery
    {
        /// <summary>
        ///     The in map for deciphering incoming query params.
        /// </summary>
        [JsonIgnore] public static readonly Dictionary<string, string[]> Map = new Dictionary<string, string[]>();

        internal static readonly JsonSerializerSettings AliasesInSettings = new JsonSerializerSettings
        {
            ContractResolver = new AliasContractResolver(Map)
        };

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderedQuery" /> class.
        /// </summary>
        public OrderedQuery()
        {
        }

        internal OrderedQuery(OrderedQuery query)
        {
            Map.TryAdd("OrderBy", new[] { "$OrderBy", "SortBy", "$SortBy", "Sort", "$Sort" });

            if (!query.OrderByAlias.IsEmptyNullOrWhiteSpace())
                OrderByAlias = query.OrderByAlias;

            OrderBy = query.OrderBy;
        }

        /// <summary>
        ///     Gets or sets the order by.
        /// </summary>
        /// <value>The order by.</value>
        [FromQuery(Name = "orderBy")]
        public string OrderBy { get; set; }

        [JsonProperty]
        internal string OrderByAlias { get; set; } = "$orderBy";


        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return BuildQuery().ToString();
        }

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        protected virtual QueryBuilder BuildQuery()
        {
            var qb = new QueryBuilder();

            if (!OrderBy.IsEmptyNullOrWhiteSpace())
                qb.Add(OrderByAlias.ToCamelCase(), OrderBy);

            return qb;
        }
    }
}
