namespace APIBlox.AspNetCore.Contracts
{
    /// <summary>
    ///     marker Interface IDynamicController
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    public interface IDynamicController<in TRequest>
    {
    }

    /// <summary>
    ///     Marker Interface IDynamicController
    ///     <para>
    ///         Use when your controller action returns a different result than the
    ///         incoming request (IE: POST) and you want to enforce a pattern.
    ///     </para>
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <typeparam name="TResponse">The type of the t response.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IDynamicController<in TRequest, in TResponse, in TId>
        where TResponse : IResource<TId>
    {
    }
}
