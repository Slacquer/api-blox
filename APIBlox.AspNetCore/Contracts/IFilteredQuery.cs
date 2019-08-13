namespace APIBlox.AspNetCore.Contracts
{
    /// <summary>
    ///     Interface IFilteredQuery
    /// </summary>
    public interface IFilteredQuery : IOrderedQuery, IProjectedQuery
    {
        /// <summary>
        ///     Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        string Filter { get; set; }
    }
}
