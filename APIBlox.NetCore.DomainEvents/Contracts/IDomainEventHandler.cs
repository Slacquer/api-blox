using System.Threading;
using System.Threading.Tasks;

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
        /// Handles the event asynchronous.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task HandleEventAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
