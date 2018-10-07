#region -    Using Statements    -

using System;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#endregion

namespace APIBlox.NetCore
{
    /// <inheritdoc cref="IDomainEventsDispatcher" />
    /// <summary>
    ///     Class DefaultDomainEventsDispatcher.
    /// </summary>
    /// <seealso cref="T:APIBlox.NetCore.Contracts.IDomainEventsDispatcher" />
    internal class DefaultDomainEventsDispatcher : IDomainEventsDispatcher
    {
        #region -    Fields    -

        private readonly ILogger<DefaultDomainEventsDispatcher> _log;

        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region -    Constructors    -

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.NetCore.DefaultDomainEventsDispatcher" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public DefaultDomainEventsDispatcher(
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider
        )
        {
            _log = loggerFactory.CreateLogger<DefaultDomainEventsDispatcher>();
            _serviceProvider = serviceProvider;
        }

        #endregion

        /// <summary>
        ///     publish events as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the t domain event.</typeparam>
        /// <param name="events">The events.</param>
        /// <returns>Task.</returns>
        /// <inheritdoc />
        public async Task PublishEventsAsync<TDomainEvent>(params TDomainEvent[] events)
            where TDomainEvent : class, IDomainEvent
        {
            foreach (var ev in events)
            {
                // Find the correct handlers, that has a generic arg that matches the event type.
                var type = typeof(IDomainEventHandler<>).MakeGenericType(ev.GetType());
                var handlers = _serviceProvider.GetServices(type).ToList();

                _log.LogInformation(() => $"Found {handlers.Count} handler(s) for Type {type}");

                foreach (var handler in handlers)
                {
                    _log.LogInformation(() => $"Calling HandleEvent {handler}");

                    await ((Task) handler.GetType().GetMethod("HandleEvent")
                        .Invoke(handler, new object[] {ev})).ConfigureAwait(false);
                }
            }
        }
    }
}
