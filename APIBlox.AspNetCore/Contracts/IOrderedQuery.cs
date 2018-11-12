namespace APIBlox.AspNetCore.Contracts
{
    /// <summary>
    ///     Interface IOrderedQuery
    /// </summary>
    public interface IOrderedQuery
    {
        /// <summary>
        ///     Gets or sets the order by.
        /// </summary>
        /// <value>The order by.</value>
        string OrderBy { get; set; }
    }
}
