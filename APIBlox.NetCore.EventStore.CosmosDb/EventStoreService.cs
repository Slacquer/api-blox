#region -    Using Statements    -

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Exceptions;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Models;
using APIBlox.NetCore.Options;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

#endregion

namespace APIBlox.NetCore
{
    [InjectableService(ServiceLifetime = ServiceLifetime.Singleton)]
    internal class EventStoreService<TAggregate> : ReadOnlyEventStoreService<TAggregate>, IEventStoreService<TAggregate>
        where TAggregate : class
    {
        #region -    Fields    -

        private readonly string _bulkInsertFilePath;
        

        #endregion

        #region -    Constructors    -

        public EventStoreService(IDocumentClient client, IOptions<EventStoreOptions> options)
            : base(client, options)
        {
            var opt = options.Value;
            
            _bulkInsertFilePath = opt.BulkInsertFilePath;

            if (!File.Exists(_bulkInsertFilePath))
                throw new ArgumentException("Bulk insert file does not exist!", nameof(opt.BulkInsertFilePath));

            
            CreateBulkInsertSprocIfNotExistsAsync().Wait();
        }

        #endregion

        public async Task<ulong> WriteToEventStreamAsync(string streamId,  EventModel[] events, ulong? expectedVersion = null,
            object metadata = null,
            CancellationToken cancellationToken = default
        )
        {
            if (streamId.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException("A stream id is required.", nameof(streamId));
            
            if (events is null || events.Length == 0)
                throw new ArgumentException("You must supply events.", nameof(events));

            RootDocument root;

            if (expectedVersion.HasValue)
            {
                root = await ReadRootAsync(streamId, cancellationToken);

                if (root.Version != expectedVersion)
                    throw new DataConcurrencyException(
                        $"Expected stream '{streamId}' to have version {expectedVersion.Value} but is {root.Version}."
                    );
            }
            else
            {
                root = new RootDocument
                {
                    PartitionBy = streamId,
                    StreamId = streamId
                };
            }

            var curVersion = root.Version;
            root.Version += (ulong)events.Length;

            if (metadata != null)
            {
                root.Metadata = metadata;
                root.MetadataType = metadata.GetType().AssemblyQualifiedName;
            }

            var docs = new List<DocumentBase> { root };

            for (ulong i = 0; i < (ulong)events.Length; i++)
                docs.Add(BuildEventDoc(events[i], streamId, ++curVersion));

            await BulkInsertEventsAsync(streamId, docs, cancellationToken);

            return root.Version;
        }
        
        public async Task DeleteEventStreamAsync(string streamId, CancellationToken cancellationToken = default)
        {
            var key = MakeKey(streamId);
            var feedOptions = new FeedOptions { PartitionKey = key };

            var query = MakeCamelCase(DbClient.CreateDocumentQuery<DocumentBase>(DocCollectionUri, feedOptions)
                        .Where(x => x.StreamId == streamId),
                    DocCollectionUri,
                    feedOptions
                )
                .AsDocumentQuery();

            while (query.HasMoreResults)
            {
                var page = await query.ExecuteNextAsync<Document>(cancellationToken);

                foreach (var document in page)
                    await DbClient.DeleteDocumentAsync(document.SelfLink, new RequestOptions { PartitionKey = key }, cancellationToken);
            }
        }

        public async Task CreateSnapshotAsync(string streamId, ulong expectedVersion, object snapshot,
            object metadata = null,
            bool deleteOlderSnapshots = false, CancellationToken cancellationToken = default
        )
        {
            var key = MakeKey(streamId);
            var root = await ReadRootAsync(streamId, cancellationToken);

            if (root.Version != expectedVersion)
                throw new DataConcurrencyException(
                    $"Expected stream '{streamId}' to have version {expectedVersion} but is {root.Version}."
                );

            var document = BuildSnapShotDoc(snapshot, metadata, expectedVersion, streamId);

            await DbClient.UpsertDocumentAsync(DocCollectionUri,
                document,
                new RequestOptions { PartitionKey = key },
                true,
                cancellationToken
            );

            if (deleteOlderSnapshots)
                await DeleteSnapshotsAsync(streamId, expectedVersion, cancellationToken);
        }

        public async Task DeleteSnapshotsAsync(string streamId, ulong olderThanVersion,
            CancellationToken cancellationToken = default
        )
        {
            await ReadRootAsync(streamId, cancellationToken);

            var key = MakeKey(streamId);
            var feedOptions = new FeedOptions { PartitionKey = key };

            var query = MakeCamelCase(DbClient.CreateDocumentQuery<SnapshotDocument>(DocCollectionUri, feedOptions)
                        .Where(x => x.StreamId == streamId)
                        .Where(x => x.DocumentType == DocumentType.Snapshot)
                        .Where(x => x.Version < olderThanVersion),
                    DocCollectionUri,
                    feedOptions
                )
                .AsDocumentQuery();

            while (query.HasMoreResults)
            {
                var page = await query.ExecuteNextAsync<Document>(cancellationToken);

                foreach (var document in page)
                {
                    await DbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, document.Id),
                        new RequestOptions { PartitionKey = key },
                        cancellationToken
                    );
                }
            }
        }


        private async Task<int> BulkInsertEventsAsync(string streamId, List<DocumentBase> docs,
            CancellationToken cancellationToken = default
        )
        {
            var key = MakeKey(streamId);
            
            var ret = await DbClient.ExecuteStoredProcedureAsync<int>(
                UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, "bulkInsert"),
                new RequestOptions { EnableScriptLogging = true, PartitionKey = key },
                cancellationToken,
                nameof(streamId),
                streamId,
                docs
            );

            return ret;
        }

        private static EventDocument BuildEventDoc(EventModel @event, string streamId, ulong streamVersion)
        {
            var document = new EventDocument
            {
                PartitionBy = streamId,
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

        private static SnapshotDocument BuildSnapShotDoc(object snapshot, object metadata, ulong version, string streamId)
        {
            var document = new SnapshotDocument
            {
                PartitionBy = streamId,
                StreamId = streamId,
                Version = version,
                SnapshotType = snapshot.GetType().AssemblyQualifiedName,
                SnapshotData = snapshot
            };

            if (metadata != null)
            {
                document.Metadata = metadata;
                document.MetadataType = metadata.GetType().AssemblyQualifiedName;
            }

            return document;
        }

        

        private async Task CreateBulkInsertSprocIfNotExistsAsync()
        {
            var exists = await DbClient.CreateStoredProcedureQuery(DocCollectionUri)
                .ToAsyncEnumerable()
                .Any(q => q.Id == "bulkInsert");

            if (exists)
                return;

            var sProc = File.ReadAllText(_bulkInsertFilePath);

            var spDef = new StoredProcedure
            {
                Id = "bulkInsert",
                Body = sProc
            };

            await DbClient.CreateStoredProcedureAsync(DocCollectionUri, spDef);
        }

        
    }
}
