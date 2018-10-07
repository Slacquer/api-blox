#region -    Using Statements    -

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#endregion

namespace APIBlox.NetCore
{
    /// <inheritdoc cref="IQueuedDomainEventsDispatcher" />
    /// <summary>
    ///     Class QueuedDomainEventsDispatcher.
    /// </summary>
    /// <seealso cref="T:APIBlox.NetCore.Contracts.IQueuedDomainEventsDispatcher" />
    internal class QueuedDomainEventsDispatcher : IQueuedDomainEventsDispatcher
    {
        #region -    Fields    -

        private readonly Queue<IDomainEvent> _eventsQueue = new Queue<IDomainEvent>();
        private readonly ILogger<QueuedDomainEventsDispatcher> _log;
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region -    Constructors    -

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.NetCore.QueuedDomainEventsDispatcher" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public QueuedDomainEventsDispatcher(
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider
        )
        {
            _log = loggerFactory.CreateLogger<QueuedDomainEventsDispatcher>();
            _serviceProvider = serviceProvider;
        }

        #endregion

        /// <summary>
        ///     Adds an event to the queue.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the t domain event.</typeparam>
        /// <param name="event">The event.</param>
        /// <returns>Task.</returns>
        /// <inheritdoc />
        public void AddEvent<TDomainEvent>(TDomainEvent @event)
            where TDomainEvent : class, IDomainEvent
        {
            _eventsQueue.Enqueue(@event);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Publishes all events.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task PublishEventsAsync(bool whenAll = true)
        {
            var tasks = new List<Task>();
            var items = _eventsQueue.ToList();

            _eventsQueue.Clear();

            foreach (var domainEvent in items)
            {
                // Find the correct handlers, that has a generic arg that matches the event type.
                var type = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var handlers = _serviceProvider.GetServices(type).ToList();

                _log.LogInformation(() => $"Found {handlers.Count} handler(s) for Type {type}");

                foreach (var handler in handlers)
                {
                    var task = (Task) handler.GetType().GetMethod("HandleEvent")
                        .Invoke(handler, new object[] {domainEvent});

                    _log.LogInformation(() => $"Calling HandleEvent {handler}");

                    if (whenAll)
                        tasks.Add(task);
                    else
                        await task.ConfigureAwait(false);
                }
            }

            if (whenAll)
                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
        }
    }
}
