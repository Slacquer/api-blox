﻿using System.Threading.Tasks;

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Interface IQueuedDomainEventsDispatcher
    /// </summary>
    public interface IQueuedDomainEventsDispatcher
    {
        /// <summary>
        ///     Adds an event to the queue.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the t domain event.</typeparam>
        /// <param name="domainEvent">The domain event.</param>
        /// <returns>Task.</returns>
        void AddEvent<TDomainEvent>(TDomainEvent domainEvent)
            where TDomainEvent : class, IDomainEvent;

        /// <summary>
        ///     Publishes all events, either one at a time or using <see cref="Task.WhenAll(Task[])" />.
        /// </summary>
        /// <param name="whenAll">if set to <c>true</c> [when all].</param>
        /// <returns>Task.</returns>
        Task PublishEventsAsync(bool whenAll = true);
    }
}
