using APIBlox.AspNetCore.Attributes;
using APIBlox.AspNetCore.Contracts;
using Microsoft.AspNetCore.Http.Extensions;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class PaginationQuery.
    ///     Implements the <see cref="APIBlox.AspNetCore.Types.Query" />
    ///     Implements the <see cref="APIBlox.AspNetCore.Contracts.IPaginationQuery" />
    ///     <para>
    ///         Be sure to also call the AddFromQueryWithAlternateNamesBinder Mvc/MvcCore
    ///         builder extension method to allow alternate names to be used.
    ///     </para>
    ///     <para>
    ///         Alternates to Skip = $Skip, Offset, $Offset
    ///     </para>
    ///     <para>
    ///         Alternates to Top = Limit, $Limit, Take, $Take
    ///     </para>
    ///     <para>
    ///         Alternates to RunningCount = Rc, Count, $Count, $RunningCount
    ///     </para>
    /// </summary>
    public class PaginationQuery : Query, IPaginationQuery
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
        [FromQueryWithAlternateNames("runningCount", "$Rc", "Rc", "Count", "$Count", "$RunningCount")]
        public int? RunningCount { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the skip amount.
        /// </summary>
        /// <value>The skip.</value>
        [FromQueryWithAlternateNames("skip", "$Skip", "Offset", "$Offset")]
        public int? Skip { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the top amount.
        /// </summary>
        /// <value>The top.</value>
        [FromQueryWithAlternateNames("top", "$Top", "Limit", "$Limit", "Take", "$Take")]
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
