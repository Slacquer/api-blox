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
        where TModel : class, IEventStoreDocument
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

            Expression<Func<IEventStoreDocument, bool>> predicate = e =>
                fromVersion.HasValue && fromVersion > 0
                    ? e.StreamId == streamId && e.Version >= fromVersion
                    : e.StreamId == streamId;

            var results = (await Repository.GetAsync<string>(predicate, cancellationToken)).ToList();

            if (results.Count == 0)
                return null;

            var docs = new List<EventStoreDocument>();

            foreach (var document in results)
            {
                var doc = EventStoreDocument.Parse(document, Repository.JsonSettings);

                docs.Add(doc);

                if (doc is RootDocument && !includeEvents)
                    break;

                if (doc is SnapshotDocument && fromVersion is null)
                    break;
            }

            if (docs.Count == 0)
                return null;

            var rootDoc = docs.FirstOrDefault(d => d.DocumentType == DocumentType.Root);

            if (rootDoc is null)
                return null;

            object metadata = null;

            if (!string.IsNullOrEmpty(rootDoc.MetadataType))
                metadata = rootDoc.Metadata;

            var events = docs.OfType<EventDocument>()
                .Select(BuildEventDocument).Reverse()
                .ToArray();

            var snapshot = fromVersion is null ? docs.OfType<SnapshotDocument>().Select(BuildSnapshotDocument).FirstOrDefault() : null;

            return new EventStreamModel
            {
                StreamId = streamId,
                Version = rootDoc.Version,
                TimeStamp = rootDoc.TimeStamp,
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

            return ret;//(RootDocument)EventStoreDocument.Parse(ret.ToString(), Repository.JsonSettings);
        }


        private static EventModel BuildEventDocument(EventDocument document)
        {
            object metadata = null;

            if (!string.IsNullOrEmpty(document.MetadataType))
                metadata = document.Metadata;

            object body = null;

            if (!string.IsNullOrEmpty(document.EventType))
                body = document.EventData;

            return new EventModel { Data = body, Version = document.Version, TimeStamp = document.TimeStamp, Metadata = metadata };
        }

        private static SnapshotModel BuildSnapshotDocument(SnapshotDocument document)
        {
            object metadata = null;

            if (!string.IsNullOrEmpty(document.MetadataType))
                metadata = document.Metadata;

            return new SnapshotModel
            {
                Data = document.SnapshotData,
                Metadata = metadata,
                Version = document.Version,
                TimeStamp = document.TimeStamp
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
