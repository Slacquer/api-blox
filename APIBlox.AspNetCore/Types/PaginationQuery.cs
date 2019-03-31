using APIBlox.AspNetCore.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Types
{
    /// <inheritdoc cref="OrderedQuery" />
    /// <summary>
    ///     Class PaginationQuery.
    /// </summary>
    public class PaginationQuery : OrderedQuery, IPaginationQuery
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.PaginationQuery" /> class.
        /// </summary>
        public PaginationQuery()
        {
            Map.TryAdd("Skip", new[] { "$Skip", "Offset", "$Offset" });
            Map.TryAdd("Top", new[] { "$Top", "Limit", "$Limit", "Take", "$Take" });
            Map.TryAdd("RunningCount", new[] { "$Rc", "Rc", "Count", "$Count", "$RunningCount" });
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the running count.  Used internally.
        /// </summary>
        /// <value>The running count.</value>
        [FromQuery(Name = "runningCount")]
        public int? RunningCount { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the skip amount.
        /// </summary>
        /// <value>The skip.</value>
        [FromQuery(Name = "skip")]
        public int? Skip { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the top amount.
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
