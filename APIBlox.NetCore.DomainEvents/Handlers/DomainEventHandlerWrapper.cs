#region -    Using Statements    -

using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.NetCore
{
    internal class DomainEventHandlerWrapper<TDomainEvent> : DomainEventHandlerBase
        where TDomainEvent : IDomainEvent
    {
        #region -    Fields    -

        private readonly IDomainEventHandler<TDomainEvent> _handler;

        #endregion

        #region -    Constructors    -

        public DomainEventHandlerWrapper(IDomainEventHandler<TDomainEvent> handler)
        {
            _handler = handler;
        }

        #endregion

        public override Task HandleEventAsync(IDomainEvent domainEvent)
        {
            return _handler.HandleEventAsync((TDomainEvent) domainEvent);
        }
    }
}
