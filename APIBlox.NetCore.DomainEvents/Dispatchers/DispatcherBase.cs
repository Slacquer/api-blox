#region -    Using Statements    -

using System;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using Microsoft.Extensions.DependencyInjection;

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.NetCore
{
    internal abstract class DispatcherBase
    {
        #region -    Fields    -

        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region -    Constructors    -

        protected DispatcherBase(
            IServiceProvider serviceProvider
        )
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

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
