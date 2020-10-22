using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIBlox.NetCore
{
    internal class ReadOnlyEventStoreService<TModel> : IReadOnlyEventStoreService<TModel>
        where TModel : class
    {
        protected readonly IEventStoreRepository<TModel> Repository;

        public ReadOnlyEventStoreService(IEventStoreRepository<TModel> repository, IEventStoreJsonSerializerSettings esSerializerSettings)
        {
            Repository = repository;
            JsonSettings = (JsonSerializerSettings)esSerializerSettings ?? new JsonSerializerSettings();

            JsonSerializer = JsonSerializer.Create(JsonSettings);
        }

        protected JsonSerializerSettings JsonSettings { get; }
        protected JsonSerializer JsonSerializer { get; }

        public async Task<(long?, DateTimeOffset?)> ReadEventStreamVersionAsync(string streamId, CancellationToken cancellationToken = default)
        {
            var root = await ReadRootAsync(streamId, cancellationToken);

            return root is null
                ? ((long?, DateTimeOffset?))(null, null)
                : (root.Version, DateTimeOffset.FromUnixTimeSeconds(root.TimeStamp));
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
            return ReadAsync(streamId, null, fromDate, toDate, null, cancellationToken);
        }

        public async Task<IEnumerable<EventStreamModel>> ReadEventStreamsAsync(
            Expression<Func<EventStoreDocument, bool>> predicate,
            int? take = null,
            int? skip = null,
            CancellationToken cancellationToken = default)
        {
            // get a list of streamId's, then build for each.
            var ids = (await Repository.GetAsync<EventStoreDocument>(
                d => d.DocumentType == DocumentType.Root,
                cancellationToken
            )).Select(r => r.Id);

            if (skip.HasValue)
                ids = ids.Skip(skip.Value);
            if (take.HasValue)
                ids = ids.Take(take.Value);

            var results = new List<EventStreamModel>();
            var tasks = new List<Task<EventStreamModel>>();

            foreach (var id in ids.ToList())
            {
                tasks.Add(ReadAsync(id, predicate: predicate, cancellationToken: cancellationToken));

                if (tasks.Count < 10)
                    continue;

                results.AddRange(await Task.WhenAll(tasks));
                tasks.Clear();
            }

            if (tasks.Any())
                results.AddRange(await Task.WhenAll(tasks));

            return results;
        }

        protected virtual EventModel BuildEventModel(EventStoreDocument document)
        {
            object data;

            try
            {
                data = JObject.FromObject(document.Data, JsonSerializer)
                    .ToObject(Type.GetType(document.DataType) ?? throw new ArgumentException(), JsonSerializer);
            }
            catch (JsonSerializationException)
            {
                data = document.Data;
            }
            catch (ArgumentException)
            {
                data = document.Data;
            }

            return new EventModel
            {
                Data = data,
                DataType = document.DataType
            };
        }

        protected virtual SnapshotModel BuildSnapshotModel(EventStoreDocument document)
        {
            var em = BuildEventModel(document);

            return new SnapshotModel
            {
                Data = em.Data,
                DataType = em.DataType,
                Version = document.Version
            };
        }

        protected async Task<RootDocument> ReadRootAsync(string streamId,
            CancellationToken cancellationToken = default
        )
        {
            var result = (await Repository.GetAsync<EventStoreDocument>(
                d => d.StreamId == streamId && d.DocumentType == DocumentType.Root,
                cancellationToken
            )).FirstOrDefault();

            var doc = !(result is null)
                ? new RootDocument
                {
                    DocumentType = result.DocumentType,
                    Id = result.Id,
                    StreamId = result.StreamId,
                    TimeStamp = result.TimeStamp,
                    Version = result.Version
                }
                : null;

            return doc;
        }

        private async Task<EventStreamModel> ReadAsync(string streamId, long? fromVersion = null, DateTimeOffset? fromDate = null,
            DateTimeOffset? toDate = null,
            Expression<Func<EventStoreDocument, bool>> predicate = null,
            CancellationToken cancellationToken = default
        )
        {
            if (streamId == null)
                throw new ArgumentNullException(nameof(streamId));

            predicate = predicate is null
                ? e => e.StreamId == streamId
                : predicate.AndAlso(e => e.StreamId == streamId);

            if (fromVersion.HasValue && fromVersion > 0)
            {
                predicate = e => e.StreamId == streamId && e.Version >= fromVersion;
            }
            else if (fromDate.HasValue)
            {
                if (!toDate.HasValue)
                    predicate = e => e.StreamId == streamId && e.TimeStamp >= fromDate.Value.Ticks;
                else
                    predicate = e => e.StreamId == streamId && e.TimeStamp >= fromDate.Value.Ticks && e.TimeStamp <= toDate.Value.Ticks;
            }

            var results = (await Repository.GetAsync<EventStoreDocument>(predicate, cancellationToken))
                .OrderByDescending(d => d.DocumentType == DocumentType.Root)
                .ThenByDescending(d => d.DocumentType == DocumentType.Snapshot)
                .ThenBy(d => d.SortOrder)
                .ToList();

            if (results.Count == 0)
                return null;

            var rootDoc = results.First(d => d.DocumentType == DocumentType.Root);

            var snapshot = fromVersion is null
                ? results
                    .OrderByDescending(d => d.SortOrder)
                    .Where(d => d.DocumentType == DocumentType.Snapshot)
                    .Select(BuildSnapshotModel).FirstOrDefault()
                : null;

            var events = results.Where(d =>
                    d.DocumentType == DocumentType.Event
                    && (d.Version > snapshot?.Version || snapshot is null)
                )
                .Select(BuildEventModel)
                .ToArray();

            return BuildEventStreamModel(streamId, rootDoc, events, snapshot);
        }

        private static EventStreamModel BuildEventStreamModel(string streamId, EventStoreDocument rootDoc,
            IEnumerable<EventModel> events = null, SnapshotModel snapshot = null
        )
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
    }
}
