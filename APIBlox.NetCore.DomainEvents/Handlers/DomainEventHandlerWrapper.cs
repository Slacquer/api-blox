using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;

// ReSharper disable once CheckNamespace
namespace APIBlox.NetCore
{
    internal class DomainEventHandlerWrapper<TDomainEvent> : DomainEventHandlerBase
        where TDomainEvent : IDomainEvent
    {
        private readonly IDomainEventHandler<TDomainEvent> _handler;

        public DomainEventHandlerWrapper(IDomainEventHandler<TDomainEvent> handler) => _handler = handler;

        public override Task HandleEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default) => 
            _handler.HandleEventAsync((TDomainEvent) domainEvent, cancellationToken);
    }
}
