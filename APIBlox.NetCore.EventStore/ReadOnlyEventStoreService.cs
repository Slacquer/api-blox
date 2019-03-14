using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Exceptions;
using APIBlox.NetCore.Models;

namespace APIBlox.NetCore
{
    internal class ReadOnlyEventStoreService<TModel> : IReadOnlyEventStoreService<TModel>
        where TModel : class
    {
        protected readonly IEventStoreRepository Repository;

        public ReadOnlyEventStoreService(IEventStoreRepository repo)
        {
            Repository = repo;
        }

        public async Task<EventStreamModel> ReadEventStreamAsync(string streamId, long? fromVersion = null,
            bool includeEvents = false,
            CancellationToken cancellationToken = default
        )
        {
            if (streamId == null)
                throw new ArgumentNullException(nameof(streamId));

            //if (fromVersion.HasValue && fromVersion > 0)
            //    await VersionCheckAsync(streamId, fromVersion, cancellationToken);

            Expression<Func<EventStoreDocument, bool>> predicate = e => e.StreamId == streamId;

            if (fromVersion.HasValue && fromVersion > 0)
                predicate = e => e.StreamId == streamId && e.Version >= fromVersion;

            var results = (await Repository.GetAsync<EventStoreDocument>(predicate, cancellationToken))
                .OrderByDescending(d => d.DocumentType == DocumentType.Root)
                .ThenByDescending(d => d.DocumentType == DocumentType.Snapshot)

                .ThenBy(d => d.SortOrder)
                .ToList();

            if (results.Count == 0)
                return null;

            var rootDoc = results.First();

            object metadata = null;

            if (!string.IsNullOrEmpty(rootDoc.MetadataType))
                metadata = rootDoc.Metadata;

            var snapshot = fromVersion is null ? results
                .OrderByDescending(d => d.SortOrder)
                .Where(d => d.DocumentType == DocumentType.Snapshot)
                .Select(BuildSnapshotModel).FirstOrDefault() : null;

            var events = results.Where(d => d.DocumentType == DocumentType.Event && (d.Version > snapshot?.Version || snapshot is null))
                .Select(BuildEventModel)
                .ToArray();

            return new EventStreamModel
            {
                StreamId = streamId,
                Version = rootDoc.Version,
                Metadata = metadata,
                Events = events.ToArray(),
                Snapshot = snapshot
            };
        }

        protected async Task<RootDocument> ReadRootAsync(string streamId,
            CancellationToken cancellationToken = default)
        {
            var ret = (await Repository.GetAsync<RootDocument>(
                d => d.StreamId == streamId && d.DocumentType == DocumentType.Root,
                cancellationToken
            )).FirstOrDefault();

            if (ret is null)
                throw new DataAccessException($"Stream '{streamId}' wasn't found");

            return ret;
        }


        private static EventModel BuildEventModel(EventStoreDocument document)
        {
            object metadata = null;

            if (!string.IsNullOrEmpty(document.MetadataType))
                metadata = document.Metadata;

            return new EventModel
            {
                Data = document.Data,
                Version = document.Version,
                Metadata = metadata
            };
        }

        private static SnapshotModel BuildSnapshotModel(EventStoreDocument document)
        {
            object metadata = null;

            if (!string.IsNullOrEmpty(document.MetadataType))
                metadata = document.Metadata;

            return new SnapshotModel
            {
                Data = document.Data,
                Metadata = metadata,
                Version = document.Version
            };
        }

        //private async Task VersionCheckAsync(string streamId,
        //    long? expectedVersion, CancellationToken cancellationToken
        //)
        //{
        //    var root = await ReadRootAsync(streamId, cancellationToken);

        //    // if for whatever reason, the incoming version is greater than what is stored then something is a miss...
        //    if (root.Version < expectedVersion)
        //        throw new DocumentConcurrencyException(
        //            $"Provided version:{expectedVersion} for stream '{streamId}' is greater than the event source version:{root.Version}!"
        //        );
        //}
    }
}
