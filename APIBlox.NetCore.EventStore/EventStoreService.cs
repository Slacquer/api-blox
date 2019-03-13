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
    internal class EventStoreService<TModel> : ReadOnlyEventStoreService<EventStoreDocument>, IEventStoreService<TModel>
        where TModel : class
    {
        public EventStoreService(IEventStoreRepository repo)
            : base(repo)
        {
        }

        public async Task<IEventStoreDocument> WriteToEventStreamAsync(string streamId, EventModel[] events, 
            long? expectedVersion = null, object metadata = null,
            CancellationToken cancellationToken = default
        )
        {
            if (streamId.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException("A stream id is required.", nameof(streamId));

            if (events is null || events.Length == 0)
                throw new ArgumentException("You must supply events.", nameof(events));

            var updating = expectedVersion.HasValue;
            RootDocument root;
            var docs = new List<EventStoreDocument>();

            if (updating)
            {
                root = await ReadRootAsync(streamId, cancellationToken);

                if (root.Version != expectedVersion)
                    throw new DocumentConcurrencyException(
                        $"Expected stream '{streamId}' to have version {expectedVersion.Value} but is {root.Version}."
                    );
            }
            else
            {
                root = new RootDocument
                {
                    StreamId = streamId
                };

                docs.Add(root);
            }

            var curVersion = root.Version;
            root.Version += events.Length;

            if (metadata != null)
            {
                root.Metadata = metadata;
                root.MetadataType = metadata.GetType().AssemblyQualifiedName;
            }

            for (long i = 0; i < events.Length; i++)
                docs.Add(BuildEventDoc(events[i], streamId, ++curVersion));

            await Repository.AddAsync(docs.ToArray(), cancellationToken);

            if (updating)
                await Repository.UpdateAsync(root, cancellationToken);

            return root;
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

            await Repository.AddAsync(new[] {doc}, cancellationToken);

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
            long streamVersion)
        {
            var document = new EventDocument
            {
                StreamId = streamId,
                Version = streamVersion,
                EventType = @event.Data.GetType().AssemblyQualifiedName,
                EventData = @event.Data
            };

            if (@event.Metadata != null)
            {
                document.MetadataType = @event.Metadata.GetType().AssemblyQualifiedName;
                document.Metadata = @event.Metadata;
            }

            return document;
        }

        private static SnapshotDocument BuildSnapShotDoc(string streamId, SnapshotModel snapshot, 
            long version)
        {
            var document = new SnapshotDocument
            {
                StreamId = streamId,
                Version = version,
                SnapshotType = snapshot.Data.GetType().AssemblyQualifiedName,
                SnapshotData = snapshot.Data
            };

            if (snapshot.Metadata != null)
            {
                document.Metadata = snapshot.Metadata;
                document.MetadataType = snapshot.Metadata.GetType().AssemblyQualifiedName;
            }

            return document;
        }
    }
}
