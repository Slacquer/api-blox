using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;

// ReSharper disable once CheckNamespace
namespace APIBlox.NetCore
{
    internal abstract class DomainEventHandlerBase
    {
        /// <summary>
        ///     Handles the event.
        /// </summary>
        /// <param name="domainEvent">The event.</param>
        /// <returns>Task.</returns>
        public abstract Task HandleEventAsync(IDomainEvent domainEvent);
    }
}
