using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Models;

namespace APIBlox.NetCore
{
    internal class ReadOnlyEventStoreService<TModel> : IReadOnlyEventStoreService<TModel>
        where TModel : class
    {
        protected bool UsingCompression { get; private set; }
        protected readonly IEventStoreRepository<TModel> Repository;

        public ReadOnlyEventStoreService(IEventStoreRepository<TModel> repo, bool useCompression)
        {
            Repository = repo;
            UsingCompression = useCompression;
        }


        public async Task<EventStreamModel> ReadEventStreamAsync(string streamId, long? fromVersion = null,
            bool includeEvents = false,
            CancellationToken cancellationToken = default
        )
        {
            if (streamId == null)
                throw new ArgumentNullException(nameof(streamId));

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

            return new EventStreamModel
            {
                StreamId = streamId,
                Version = rootDoc.Version,
                TimeStamp = DateTimeOffset.Parse(rootDoc.TimeStamp),
                Events = events.ToArray(),
                Snapshot = snapshot
            };
        }

        protected async Task<RootDocument> ReadRootAsync(string streamId,
            CancellationToken cancellationToken = default)
        {
            var result = await Repository.GetAsync<RootDocument>(
                d => d.StreamId == streamId && d.DocumentType == DocumentType.Root,
                cancellationToken
            );

            return result.FirstOrDefault();
        }


        protected static EventModel BuildEventModel(EventStoreDocument document)
        {
            return new EventModel
            {
                Data = document.Data,
                DataType = document.DataType
            };
        }

        private static SnapshotModel BuildSnapshotModel(EventStoreDocument document)
        {
            return new SnapshotModel
            {
                Data = document.Data,
                DataType = document.DataType,
                Version = document.Version
            };
        }
    }
}
