using System;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace APIBlox.NetCore
{
    /// <inheritdoc cref="IDomainEventsDispatcher" />
    /// <summary>
    ///     Class DomainEventsDispatcher.
    /// </summary>
    /// <seealso cref="T:APIBlox.NetCore.Contracts.IDomainEventsDispatcher" />
    internal class DomainEventsDispatcher : DispatcherBase, IDomainEventsDispatcher
    {
        private readonly ILogger<DomainEventsDispatcher> _log;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.NetCore.DefaultDomainEventsDispatcher" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public DomainEventsDispatcher(
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider
        )
            : base(serviceProvider)
        {
            _log = loggerFactory.CreateLogger<DomainEventsDispatcher>();
        }

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
                await ExecuteHandlers(ev,
                    async handlerTask =>
                    {
                        _log.LogInformation(() => $"Calling HandleEvent {handlerTask}");

                        await handlerTask.ConfigureAwait(false);
                    }
                );
            }
            
        }
    }
}
