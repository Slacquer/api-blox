namespace APIBlox.AspNetCore.Contracts
{
    /// <summary>
    ///     Interface IProjectedQuery
    /// </summary>
    public interface IProjectedQuery : IQuery
    {
        /// <summary>
        ///     Gets or sets the select.
        /// </summary>
        /// <value>The select.</value>
        string Select { get; set; }
    }
}
