using System;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace APIBlox.NetCore
{
    internal abstract class DispatcherBase
    {
        private readonly IServiceProvider _serviceProvider;

        protected DispatcherBase(
            IServiceProvider serviceProvider
        )
        {
            _serviceProvider = serviceProvider;
        }

        protected void ExecuteHandlers(IDomainEvent de, Action<Task> callback)
        {
            // Find the correct handlers, that has a generic arg that matches the event type.
            // Create a wrapper to prevent from invoking magic string methods.
            var deType = de.GetType();
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(deType);
            var handlerWrapperType = typeof(DomainEventHandlerWrapper<>).MakeGenericType(deType);
            var handlers = _serviceProvider.GetServices(handlerType).ToList();

            var wrapped = handlers.Select(h =>
                (DomainEventHandlerBase) Activator.CreateInstance(handlerWrapperType, h)
            );

            foreach (var dh in wrapped)
                callback(dh.HandleEventAsync(de));
        }
    }
}
