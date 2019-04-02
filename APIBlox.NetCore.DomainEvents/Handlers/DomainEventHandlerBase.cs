using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;

// ReSharper disable once CheckNamespace
namespace APIBlox.NetCore
{
    internal abstract class DomainEventHandlerBase
    {
        /// <summary>
        ///     Handles the event asynchronous.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public abstract Task HandleEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
