﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Exceptions;
using APIBlox.NetCore.Models;
using Examples.Events;
using Newtonsoft.Json;

namespace Examples.AggregateModels
{
    /// <summary>
    ///     Class Aggregate
    /// </summary>
    public class Aggregate<TAggregate>
        where TAggregate : class
    {
        private readonly IEventStoreService<TAggregate> _es;

        private readonly IDictionary<Type, MethodInfo> _whenMethods;
        private EventStreamModel _myEventStream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Aggregate{TAggregate}" /> class.
        /// </summary>
        /// <param name="eventStoreService">The event store service.</param>
        /// <param name="streamId">The stream identifier.</param>
        public Aggregate(IEventStoreService<TAggregate> eventStoreService, string streamId)
        {
            _es = eventStoreService;

            _whenMethods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == "When")
                .Where(m => m.GetParameters().Length == 1)
                .ToDictionary(m => m.GetParameters().First().ParameterType, m => m);

            StreamId = streamId;
        }

        /// <summary>
        ///     Gets the stream identifier.
        /// </summary>
        /// <value>The stream identifier.</value>
        public string StreamId { get; private set; }

        /// <summary>
        ///     Gets the domain events.
        /// </summary>
        /// <value>The domain events.</value>
        [JsonIgnore]
        public List<object> DomainEvents { get; } = new();

        /// <summary>
        ///     Gets the aggregate identifier.
        /// </summary>
        /// <value>The aggregate identifier.</value>
        public Guid AggregateId { get; private set; }

        /// <summary>
        ///     Gets some value.
        /// </summary>
        /// <value>Some value.</value>
        public string SomeValue { get; private set; }

        /// <summary>
        ///     Gets my version.
        /// </summary>
        /// <value>My version.</value>
        public long MyVersion { get; private set; }

        /// <summary>
        ///     Adds some value.
        /// </summary>
        /// <param name="someValue">Some value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="EventStoreConcurrencyException">Aggregate with stream id {_streamId}</exception>
        public async Task AddSomeValueAsync(string someValue, CancellationToken cancellationToken)
        {
            await BuildAsync(false, cancellationToken);

            if (_myEventStream is not null)
                throw new EventStoreConcurrencyException($"Aggregate with stream id {StreamId} already exists!");

            _myEventStream = new EventStreamModel();

            AggregateId = Guid.NewGuid();

            // Validate and such
            SomeValue = someValue;

            DomainEvents.Add(new SomeValueAdded(AggregateId, SomeValue));
        }

        /// <summary>
        ///     Updates some value.
        /// </summary>
        /// <param name="someValue">Some value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="EventStoreConcurrencyException">Aggregate with stream id {_streamId}</exception>
        public async Task UpdateSomeValueAsync(string someValue, CancellationToken cancellationToken)
        {
            await BuildAsync(false, cancellationToken);

            if (_myEventStream is null)
                throw new EventStoreConcurrencyException($"Aggregate with stream id {StreamId} not found!");

            // Validate and such
            SomeValue = someValue;

            DomainEvents.Add(new SomeValueChanged(AggregateId, SomeValue));
        }

        /// <summary>
        ///     publish changes as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task PublishChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await _es.WriteToEventStreamAsync(StreamId,
                DomainEvents.Select(e => new EventModel { Data = e }).ToArray(),
                null,
                _myEventStream.Version > 0 ? _myEventStream.Version : null,
                cancellationToken
            );

            MyVersion = result.Version;

            if (result.Version % 10 == 0)
                await _es.CreateSnapshotAsync(StreamId,
                    result.Version,
                    new SnapshotModel { Data = this },
                    cancellationToken: cancellationToken
                );

            _myEventStream = result;
        }

        /// <summary>
        ///     Builds the specified fail not found.
        /// </summary>
        /// <param name="failNotFound">if set to <c>true</c> [fail not found].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="EventStoreNotFoundException">StreamId {_streamId}</exception>
        public async Task BuildAsync(bool failNotFound = false, CancellationToken cancellationToken = default)
        {
            if (_myEventStream is not null)
                return;

            _myEventStream = await _es.ReadEventStreamAsync(StreamId, cancellationToken);

            if (_myEventStream is null)
            {
                if (failNotFound)
                    throw new EventStoreNotFoundException($"StreamId {StreamId} not found");

                return;
            }

            if (_myEventStream.Snapshot is not null)
            {
                var data = (Aggregate<TAggregate>)_myEventStream.Snapshot.Data;

                SomeValue = data.SomeValue;
                AggregateId = data.AggregateId;
            }

            ApplyPreviousEvents(_myEventStream.Events);

            MyVersion = _myEventStream.Version;
        }

        /// <summary>
        ///     Deletes me.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public Task DeleteMeAsync(CancellationToken cancellationToken)
        {
            return _es.DeleteEventStreamAsync(StreamId, cancellationToken);
        }

        private void When(SomeValueAdded e)
        {
            SomeValue = e.SomeValue;
            AggregateId = e.AggregateId;
        }

        private void When(SomeValueChanged e)
        {
            SomeValue = e.SomeValue;
            AggregateId = e.AggregateId;
        }

        private void ApplyPreviousEvents(IEnumerable<EventModel> events)
        {
            foreach (var ev in events.Select(ev => ev.Data))
                InvokeEventOptional(ev);
        }

        private void InvokeEventOptional(object @event)
        {
            var type = @event.GetType();

            if (!_whenMethods.TryGetValue(type, out var info))
            {
                var s = $"Failed to locate {GetType().Name}.When({type.Name})";
                throw new InvalidOperationException(s);
            }

            info.Invoke(this, new[] { @event });
        }
    }
}
