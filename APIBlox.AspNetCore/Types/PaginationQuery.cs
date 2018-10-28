using System.Collections.Generic;
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
        private const string RunningCountParam = "$runningCount";
        private const string SkipParam = "$skip";
        private const string TopParam = "$top";

        /// <summary>
        ///     The in map for deciphering incoming query params.
        /// </summary>
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
        }

        /// <summary>
        ///     Gets or sets the running count.
        /// </summary>
        /// <value>The running count.</value>
        public int? RunningCount { get; set; }

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
        ///     Gets or sets the undefined parameters.
        /// </summary>
        /// <value>The other.</value>
        [JsonExtensionData]
        public IDictionary<string, JToken> Undefined { get; set; }

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        protected virtual QueryBuilder BuildQuery()
        {
            var qb = new QueryBuilder();

            if (Skip.HasValue)
                qb.Add(SkipParam, Skip.Value.ToString());

            if (Top.HasValue)
                qb.Add(TopParam, Top.Value.ToString());

            if (RunningCount.HasValue)
                qb.Add(RunningCountParam, RunningCount.ToString());

            if (Undefined is null)
                return qb;

            foreach (var p in Undefined)
                qb.Add(p.Key, p.Value.ToString());

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
