using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Models;
using Newtonsoft.Json;

namespace APIBlox.NetCore
{
    internal class ReadOnlyEventStoreService<TModel> : IReadOnlyEventStoreService<TModel>
        where TModel : class
    {
        protected readonly IEventStoreRepository<TModel> Repository;
        protected JsonSerializerSettings JsonSettings { get; }

        public ReadOnlyEventStoreService(IEventStoreRepository<TModel> repository, JsonSerializerSettings serializerSettings)
        {
            Repository = repository;
            JsonSettings = serializerSettings ?? new JsonSerializerSettings();
        }

        public async Task<(long, DateTimeOffset)> ReadEventStreamVersionAsync(string streamId, CancellationToken cancellationToken = default)
        {
            var root = await ReadRootAsync(streamId, cancellationToken);

            if (root is null)
                return (0, default(DateTime));

            return (root.Version, DateTimeOffset.FromUnixTimeSeconds(root.TimeStamp));
        }

        public Task<EventStreamModel> ReadEventStreamAsync(string streamId, CancellationToken cancellationToken = default)
        {
            return ReadAsync(streamId, cancellationToken: cancellationToken);
        }

        public Task<EventStreamModel> ReadEventStreamAsync(string streamId, long fromVersion,
            CancellationToken cancellationToken = default
        )
        {
            return ReadAsync(streamId, fromVersion, cancellationToken: cancellationToken);
        }

        public Task<EventStreamModel> ReadEventStreamAsync(string streamId, DateTimeOffset fromDate,
            DateTimeOffset? toDate = null,
            CancellationToken cancellationToken = default
        )
        {
            return ReadAsync(streamId, null, fromDate, toDate, cancellationToken);
        }


        private async Task<EventStreamModel> ReadAsync(string streamId, long? fromVersion = null, DateTimeOffset? fromDate = null,
            DateTimeOffset? toDate = null, CancellationToken cancellationToken = default)
        {
            if (streamId == null)
                throw new ArgumentNullException(nameof(streamId));

            Expression<Func<IEventStoreDocument, bool>> predicate = e => e.StreamId == streamId;

            if (fromVersion.HasValue && fromVersion > 0)
                predicate = e => e.StreamId == streamId && e.Version >= fromVersion;
            else if (fromDate.HasValue)
            {
                if (!toDate.HasValue)
                    predicate = e => e.StreamId == streamId && e.TimeStamp >= fromDate.Value.Ticks;
                else
                    predicate = e => e.StreamId == streamId && e.TimeStamp >= fromDate.Value.Ticks && e.TimeStamp <= toDate.Value.Ticks;
            }

            var results = (await Repository.GetAsync<IEventStoreDocument>(predicate, cancellationToken))
                .OrderByDescending(d => d.DocumentType == DocumentType.Root)
                .ThenByDescending(d => d.DocumentType == DocumentType.Snapshot)
                .ThenBy(d => d.SortOrder)
                .ToList();

            if (results.Count == 0)
                return null;

            var rootDoc = results.First(d => d.DocumentType == DocumentType.Root);

            var snapshot = fromVersion is null ? results
                .OrderByDescending(d => d.SortOrder)
                .Where(d => d.DocumentType == DocumentType.Snapshot)
                .Select(BuildSnapshotModel).FirstOrDefault() : null;

            var events = results.Where(d =>
                    d.DocumentType == DocumentType.Event
                    && (d.Version > snapshot?.Version || snapshot is null)
                )
                .Select(BuildEventModel)
                .ToArray();

            return BuildEventStreamModel(streamId, rootDoc, events, snapshot);
        }


        private static EventStreamModel BuildEventStreamModel(string streamId, IEventStoreDocument rootDoc,
            IEnumerable<EventModel> events = null, SnapshotModel snapshot = null)
        {
            return new EventStreamModel
            {
                StreamId = streamId,
                Version = rootDoc.Version,
                TimeStamp = DateTimeOffset.FromUnixTimeSeconds(rootDoc.TimeStamp),
                Events = events?.ToArray(),
                Snapshot = snapshot
            };
        }

        protected async Task<RootDocument> ReadRootAsync(string streamId,
            CancellationToken cancellationToken = default)
        {
            var result = await Repository.GetAsync<IEventStoreDocument>(
                d => d.StreamId == streamId && d.DocumentType == DocumentType.Root,
                cancellationToken
            );

            return (RootDocument) result.FirstOrDefault();
        }

        protected virtual EventModel BuildEventModel(IEventStoreDocument document)
        {
            var d = (EventStoreDocument) document;

            return new EventModel
            {
                Data = JsonConvert.DeserializeObject(document.Data, Type.GetType(document.DataType), JsonSettings),
                DataType = document.DataType
            };
        }

        protected virtual SnapshotModel BuildSnapshotModel(IEventStoreDocument document)
        {
            var d = (EventStoreDocument) document;

            return new SnapshotModel
            {
                Data = JsonConvert.DeserializeObject(document.Data, Type.GetType(document.DataType), JsonSettings),
                DataType = document.DataType,
                Version = document.Version
            };
        }
    }
}
