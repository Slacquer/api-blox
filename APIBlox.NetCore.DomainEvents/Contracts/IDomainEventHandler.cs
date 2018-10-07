#region -    Using Statements    -

using System.Threading.Tasks;

#endregion

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Interface IDomainEventHandler
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the t domain event.</typeparam>
    public interface IDomainEventHandler<in TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        /// <summary>
        ///     Handles the event.
        /// </summary>
        /// <param name="domainEvent">The event.</param>
        /// <returns>Task.</returns>
        Task HandleEventAsync(TDomainEvent domainEvent);
    }
}
