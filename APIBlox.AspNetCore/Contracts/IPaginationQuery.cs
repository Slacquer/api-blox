namespace APIBlox.AspNetCore.Contracts
{
    /// <inheritdoc />
    /// <summary>
    ///     Interface IPaginationQuery
    /// </summary>
    public interface IPaginationQuery : IOrderedQuery
    {
        /// <summary>
        ///     Gets or sets the running count.
        /// </summary>
        /// <value>The running count.</value>
        int? RunningCount { get; set; }

        /// <summary>
        ///     Gets or sets the skip.
        /// </summary>
        /// <value>The skip.</value>
        int? Skip { get; set; }

        /// <summary>
        ///     Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        int? Top { get; set; }
    }
}
