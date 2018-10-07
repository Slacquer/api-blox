#region -    Using Statements    -

using System.Threading.Tasks;

#endregion

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Interface IDomainEventsDispatcher
    /// </summary>
    public interface IDomainEventsDispatcher
    {
        /// <summary>
        ///     Publishes all events asynchronously.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the t domain event.</typeparam>
        /// <param name="events">The events.</param>
        /// <returns>Task.</returns>
        Task PublishEventsAsync<TDomainEvent>(params TDomainEvent[] events)
            where TDomainEvent : class, IDomainEvent;
    }
}
