namespace APIBlox.AspNetCore.Contracts
{
    /// <summary>
    ///     Marker interface
    /// </summary>
    public interface IResource
    {
    }

    /// <inheritdoc />
    /// <summary>
    ///     Interface IResource
    /// </summary>
    /// <typeparam name="TId">The type of the t id.</typeparam>
    /// <seealso cref="T:APIBlox.AspNetCore.Contracts.IResource" />
    public interface IResource<TId> : IResource
    {
        /// <summary>
        ///     Gets or sets the Id.
        /// </summary>
        /// <value>The Id.</value>
        TId Id { get; set; }
    }
}
