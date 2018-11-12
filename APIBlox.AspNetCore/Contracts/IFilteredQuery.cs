namespace APIBlox.AspNetCore.Contracts
{
    /// <inheritdoc />
    /// <summary>
    ///     Interface IFilteredQuery
    /// </summary>
    public interface IFilteredQuery : IOrderedQuery
    {
        /// <summary>
        ///     Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        string Filter { get; set; }

        /// <summary>
        ///     Gets or sets the select.
        /// </summary>
        /// <value>The select.</value>
        string Select { get; set; }
    }
}
