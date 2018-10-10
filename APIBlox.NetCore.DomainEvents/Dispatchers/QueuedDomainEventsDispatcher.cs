using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace APIBlox.NetCore
{
    internal class QueuedDomainEventsDispatcher : DispatcherBase, IQueuedDomainEventsDispatcher
    {
        private readonly Queue<IDomainEvent> _eventsQueue = new Queue<IDomainEvent>();
        private readonly ILogger<QueuedDomainEventsDispatcher> _log;

        public QueuedDomainEventsDispatcher(
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider
        )
            : base(serviceProvider)
        {
            _log = loggerFactory.CreateLogger<QueuedDomainEventsDispatcher>();
        }

        public void AddEvent<TDomainEvent>(TDomainEvent domainEvent)
            where TDomainEvent : class, IDomainEvent
        {
            _eventsQueue.Enqueue(domainEvent);
        }

        public async Task PublishEventsAsync(bool whenAll = true)
        {
            var tasks = new List<Task>();
            var events = _eventsQueue.ToList();

            _eventsQueue.Clear();

            foreach (var ev in events)
            {
                ExecuteHandlers(ev,
                    async handlerTask =>
                    {
                        _log.LogInformation(() => $"Calling HandleEvent {handlerTask}");

                        if (whenAll)
                            tasks.Add(handlerTask);
                        else
                            await handlerTask.ConfigureAwait(false);
                    }
                );
            }

            if (whenAll)
                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
        }
    }
}
