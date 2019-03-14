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

namespace Examples
{
    /// <summary>
    ///     Class MyAggregate.
    /// </summary>
    public class MyAggregate
    {
        private readonly IEventStoreService<MyAggregate> _es;

        private readonly string _streamId;
        private readonly IDictionary<Type, MethodInfo> _whenMethods;
        private EventStreamModel _myEventStream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MyAggregate" /> class.
        /// </summary>
        /// <param name="eventStoreService">The event store service.</param>
        /// <param name="streamId">The stream identifier.</param>
        public MyAggregate(IEventStoreService<MyAggregate> eventStoreService, string streamId)
        {
            _es = eventStoreService;

            _whenMethods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == "When")
                .Where(m => m.GetParameters().Length == 1)
                .ToDictionary(m => m.GetParameters().First().ParameterType, m => m);

            _streamId = streamId;
        }

        /// <summary>
        ///     Gets the domain events.
        /// </summary>
        /// <value>The domain events.</value>
        [JsonIgnore]
        public List<object> DomainEvents { get; } = new List<object>();

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
        ///     Adds some value.
        /// </summary>
        /// <param name="someValue">Some value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="DataAccessException">Aggregate with stream id {_streamId}</exception>
        public async Task AddSomeValue(string someValue, CancellationToken cancellationToken)
        {
            await Build(cancellationToken);

            if (!(_myEventStream is null))
                throw new DataAccessException($"Aggregate with stream id {_streamId} already exists!");

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
        /// <exception cref="DataAccessException">Aggregate with stream id {_streamId}</exception>
        public async Task UpdateSomeValue(string someValue, CancellationToken cancellationToken)
        {
            await Build(cancellationToken);

            if (_myEventStream is null)
                throw new DataAccessException($"Aggregate with stream id {_streamId} not found!");

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
            var result = await _es.WriteToEventStreamAsync(_streamId,
                DomainEvents.Select(e => new EventModel {Data = e}).ToArray(),
                _myEventStream.Version > 0 ? _myEventStream.Version : (long?) null,
                cancellationToken: cancellationToken
            );

            if (result.Version % 2 == 0)
                await _es.CreateSnapshotAsync(_streamId,
                    result.Version,
                    new SnapshotModel {Data = this},
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        ///     Builds the specified cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task Build(CancellationToken cancellationToken = default)
        {
            if (!(_myEventStream is null))
                return;

            _myEventStream = await _es.ReadEventStreamAsync(_streamId, includeEvents: true, cancellationToken: cancellationToken);

            if (_myEventStream is null)
                return;

            if (!(_myEventStream.Snapshot is null))
            {
                var data = (MyAggregate) _myEventStream.Snapshot.Data;

                SomeValue = data.SomeValue;
                AggregateId = data.AggregateId;
            }

            ApplyPreviousEvents(_myEventStream.Events);
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

            info.Invoke(this, new[] {@event});
        }
    }
}