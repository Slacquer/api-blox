using System.Collections.Generic;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class PaginationQuery.
    /// </summary>
    public class PaginationQuery : OrderedQuery
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PaginationQuery" /> class.
        /// </summary>
        public PaginationQuery()
        {
        }

        internal PaginationQuery(PaginationQuery query)
        {
            Map.TryAdd("Skip", new[] { "$Skip", "Offset", "$Offset" });
            Map.TryAdd("Top", new[] {"$Top", "Limit", "$Limit", "Take", "$Take"});
            Map.TryAdd("RunningCount", new[] {"$Rc", "Rc", "Count", "$Count", "$RunningCount"});

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
        [FromQuery(Name = "runningCount")]
        public int? RunningCount { get; set; }

        [JsonProperty]
        internal string RunningCountAlias { get; set; } = "$runningCount";


        /// <summary>
        ///     Gets or sets the skip.
        /// </summary>
        /// <value>The skip.</value>
        [FromQuery(Name = "skip")]
        public int? Skip { get; set; }

        [JsonProperty]
        internal string SkipAlias { get; set; } = "$skip";

        /// <summary>
        ///     Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        [FromQuery(Name = "top")]
        public int? Top { get; set; }

        [JsonProperty]
        internal string TopAlias { get; set; } = "$top";


        /// <summary>
        ///     Gets or sets the undefined parameters.
        /// </summary>
        /// <value>The other.</value>
        [JsonExtensionData]
        internal IDictionary<string, JToken> Undefined { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        protected override QueryBuilder BuildQuery()
        {
            var qb = base.BuildQuery();

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

        /// <inheritdoc />
        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents this instance if the form of a query string.
        /// </summary>
        /// <returns>A <see cref="T:System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return BuildQuery().ToString();
        }
    }
}
