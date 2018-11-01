using System.Collections.Generic;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class PaginationQuery.
    /// </summary>
    public class PaginationQuery
    {
        /// <summary>
        ///     The in map for deciphering incoming query params.
        /// </summary>
        [JsonIgnore]
        public static readonly Dictionary<string, string[]> PaginationMap = new Dictionary<string, string[]>
        {
            {"Skip", new[] {"$Skip", "Offset", "$Offset"}},
            {"Top", new[] {"$Top", "Limit", "$Limit", "Take", "$Take"}},
            {"RunningCount", new[] {"$Rc", "Rc", "Count", "$Count", "$RunningCount"}},
        };

        internal static readonly JsonSerializerSettings AliasesInSettings = new JsonSerializerSettings
        {
            ContractResolver = new AliasContractResolver(PaginationMap)
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
            Undefined = query.Undefined;

            if (!query.RunningCountAlias.IsEmptyNullOrWhiteSpace())
                RunningCountAlias = query.RunningCountAlias;

            if (!query.SkipAlias.IsEmptyNullOrWhiteSpace())
                SkipAlias = query.SkipAlias;

            if (!query.TopAlias.IsEmptyNullOrWhiteSpace())
                TopAlias = query.TopAlias;
        }

        /// <summary>
        ///     Gets or sets the running count.
        /// </summary>
        /// <value>The running count.</value>
        public int? RunningCount { get; set; }

        [JsonProperty]
        internal string RunningCountAlias { get; set; } = "$runningCount";


        /// <summary>
        ///     Gets or sets the skip.
        /// </summary>
        /// <value>The skip.</value>
        public int? Skip { get; set; }

        [JsonProperty]
        internal string SkipAlias { get; set; } = "$skip";

        /// <summary>
        ///     Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public int? Top { get; set; }

        [JsonProperty]
        internal string TopAlias { get; set; } = "$top";


        /// <summary>
        ///     Gets or sets the undefined parameters.
        /// </summary>
        /// <value>The other.</value>
        [JsonExtensionData]
        internal IDictionary<string, JToken> Undefined { get; set; }

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        protected virtual QueryBuilder BuildQuery()
        {
            var qb = new QueryBuilder();

            if (Skip.HasValue)
                qb.Add(SkipAlias.ToCamelCase(), Skip.Value.ToString());

            if (Top.HasValue)
                qb.Add(TopAlias.ToCamelCase(), Top.Value.ToString());

            if (RunningCount.HasValue)
                qb.Add(RunningCountAlias.ToCamelCase(), RunningCount.ToString());

            if (Undefined is null)
                return qb;

            foreach (var p in Undefined)
                qb.Add(p.Key.ToCamelCase(), p.Value.ToString());

            return qb;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance if the form of a query string.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return BuildQuery().ToString();
        }
    }
}
