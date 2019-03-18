using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Exceptions;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Models;

namespace APIBlox.NetCore
{
    internal class EventStoreService<TModel> : ReadOnlyEventStoreService<TModel>, IEventStoreService<TModel>
        where TModel : class
    {
        public EventStoreService(IEventStoreRepository<TModel> repo, bool useCompression)
            : base(repo, useCompression)
        {
        }

        public async Task<EventStreamModel> WriteToEventStreamAsync(string streamId, EventModel[] events,
            long? expectedVersion = null, CancellationToken cancellationToken = default
        )
        {
            if (streamId.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException("A stream id is required.", nameof(streamId));

            if (events is null || events.Length == 0)
                throw new ArgumentException("You must supply events.", nameof(events));

            var updating = expectedVersion.HasValue;

            var root = await ReadRootAsync(streamId, cancellationToken);

            if (root is null && expectedVersion.HasValue)
                throw new EventStoreNotFoundException($"Stream '{streamId}' wasn't found.");

            if (!(root is null) && !(expectedVersion.HasValue))
                throw new EventStoreConcurrencyException($"Stream '{streamId}' exists, therefore you must specify an expected version.");

            var docs = new List<EventStoreDocument>();

            if (updating)
            {
                if (root.Version != expectedVersion)
                    throw new EventStoreConcurrencyException(
                        $"Expected stream '{streamId}' to have version {expectedVersion.Value} but is {root.Version}."
                    );

                root.TimeStamp = DateTimeOffset.Now.ToString();
            }
            else
            {
                root = new RootDocument
                {
                    StreamId = streamId,
                    TimeStamp = DateTimeOffset.Now.ToString()
                };

                docs.Add(root);
            }

            var curVersion = root.Version;
            root.Version += events.Length;

            var ret = new EventStreamModel
            {
                StreamId = streamId,
                Version = root.Version,
                TimeStamp = DateTimeOffset.Parse(root.TimeStamp)
            };

            var lst = new List<EventModel>();
            for (long i = 0; i < events.Length; i++)
            {
                var eDoc = BuildEventDoc(events[i], streamId, root.TimeStamp, ++curVersion);
                docs.Add(eDoc);
                lst.Add(BuildEventModel(eDoc));
            }

            ret.Events = lst.ToArray();

            await Repository.AddAsync(docs.ToArray(), cancellationToken);

            if (updating)
                await Repository.UpdateAsync(root, cancellationToken);

            return ret;
        }

        public async Task DeleteEventStreamAsync(string streamId,
            CancellationToken cancellationToken = default)
        {
            await Repository.DeleteAsync(d => d.StreamId == streamId, cancellationToken);
        }

        public async Task CreateSnapshotAsync(string streamId, long expectedVersion,
            SnapshotModel snapshot, bool deleteOlderSnapshots = false,
            CancellationToken cancellationToken = default
        )
        {
            var doc = BuildSnapShotDoc(streamId, snapshot, expectedVersion);

            await Repository.AddAsync(new[] { doc }, cancellationToken);

            if (deleteOlderSnapshots)
                await DeleteSnapshotsAsync(streamId, expectedVersion, cancellationToken);
        }

        public async Task DeleteSnapshotsAsync(string streamId, long olderThanVersion,
            CancellationToken cancellationToken = default)
        {
            await Repository.DeleteAsync(d =>
                    d.StreamId == streamId
                    && d.DocumentType == DocumentType.Snapshot
                    && d.Version < olderThanVersion,
                cancellationToken
            );
        }


        private static EventDocument BuildEventDoc(EventModel @event, string streamId,
           string timeStamp, long streamVersion)
        {
            var document = new EventDocument
            {
                StreamId = streamId,
                Version = streamVersion,
                TimeStamp = timeStamp,
                DataType = @event.Data.GetType().AssemblyQualifiedName,
                Data = @event.Data
            };

            return document;
        }

        private static SnapshotDocument BuildSnapShotDoc(string streamId, SnapshotModel snapshot,
           long version)
        {
            var document = new SnapshotDocument
            {
                StreamId = streamId,
                Version = version,
                TimeStamp = DateTimeOffset.Now.ToString(),
                DataType = snapshot.Data.GetType().AssemblyQualifiedName,
                Data = snapshot.Data
            };

            return document;
        }
    }
}
