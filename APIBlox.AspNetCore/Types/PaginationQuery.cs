using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class PaginationQuery.
    /// </summary>
    public class PaginationQuery : OrderedQuery
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.PaginationQuery" /> class.
        /// </summary>
        public PaginationQuery()
        {
            Map.TryAdd("Skip", new[] {"$Skip", "Offset", "$Offset"});
            Map.TryAdd("Top", new[] {"$Top", "Limit", "$Limit", "Take", "$Take"});
            Map.TryAdd("RunningCount", new[] {"$Rc", "Rc", "Count", "$Count", "$RunningCount"});
        }

        /// <summary>
        ///     Gets or sets the running count.
        /// </summary>
        /// <value>The running count.</value>
        [FromQuery(Name = "runningCount")]
        public int? RunningCount { get; set; }

        /// <summary>
        ///     Gets or sets the skip.
        /// </summary>
        /// <value>The skip.</value>
        [FromQuery(Name = "skip")]
        public int? Skip { get; set; }

        /// <summary>
        ///     Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        [FromQuery(Name = "top")]
        public int? Top { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>QueryBuilder.</returns>
        protected override QueryBuilder BuildQuery()
        {
            var qb = base.BuildQuery();

            if (Skip.HasValue)
                qb.Add("skip", Skip.Value.ToString());

            if (Top.HasValue)
                qb.Add("top", Top.Value.ToString());

            if (RunningCount.HasValue)
                qb.Add("runningCount", RunningCount.ToString());

            return qb;
        }
    }
}
