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
    internal class EventStoreService<TModel> : ReadOnlyEventStoreService<EventStoreDocument>, IEventStoreService<TModel>
        where TModel : class
    {
        #region -    Constructors    -

        public EventStoreService(IEventStoreRepository repo)
            : base(repo)
        {
        }

        #endregion

        //public async Task<long> WriteToEventStreamAsync(string streamId,  EventModel[] events, long? expectedVersion = null,
        //    object metadata = null,
        //    CancellationToken cancellationToken = default
        //)
        //{
        //    if (streamId.IsEmptyNullOrWhiteSpace())
        //        throw new ArgumentException("A stream id is required.", nameof(streamId));

        //    if (events is null || events.Length == 0)
        //        throw new ArgumentException("You must supply events.", nameof(events));

        //    RootDocument root;

        //    if (expectedVersion.HasValue)
        //    {
        //        root = await ReadRootAsync(streamId, cancellationToken);

        //        if (root.Version != expectedVersion)
        //            throw new DataConcurrencyException(
        //                $"Expected stream '{streamId}' to have version {expectedVersion.Value} but is {root.Version}."
        //            );
        //    }
        //    else
        //    {
        //        root = new RootDocument
        //        {
        //            PartitionBy = streamId,
        //            StreamId = streamId
        //        };
        //    }

        //    var curVersion = root.Version;
        //    root.Version += (long)events.Length;

        //    if (metadata != null)
        //    {
        //        root.Metadata = metadata;
        //        root.MetadataType = metadata.GetType().AssemblyQualifiedName;
        //    }

        //    var docs = new List<DocumentBase> { root };

        //    for (long i = 0; i < (long)events.Length; i++)
        //        docs.Add(BuildEventDoc(events[i], streamId, ++curVersion));

        //    await BulkInsertEventsAsync(streamId, docs, cancellationToken);

        //    return root.Version;
        //}

        //public async Task DeleteEventStreamAsync(string streamId, CancellationToken cancellationToken = default)
        //{
        //    var key = MakeKey(streamId);
        //    var feedOptions = new FeedOptions { PartitionKey = key };

        //    var query = MakeCamelCase(DbClient.CreateDocumentQuery<DocumentBase>(DocCollectionUri, feedOptions)
        //                .Where(x => x.StreamId == streamId),
        //            DocCollectionUri,
        //            feedOptions
        //        )
        //        .AsDocumentQuery();

        //    while (query.HasMoreResults)
        //    {
        //        var page = await query.ExecuteNextAsync<Document>(cancellationToken);

        //        foreach (var document in page)
        //            await DbClient.DeleteDocumentAsync(document.SelfLink, new RequestOptions { PartitionKey = key }, cancellationToken);
        //    }
        //}

        //public async Task CreateSnapshotAsync(string streamId, long expectedVersion, object snapshot,
        //    object metadata = null,
        //    bool deleteOlderSnapshots = false, CancellationToken cancellationToken = default
        //)
        //{
        //    var key = MakeKey(streamId);
        //    var root = await ReadRootAsync(streamId, cancellationToken);

        //    if (root.Version != expectedVersion)
        //        throw new DataConcurrencyException(
        //            $"Expected stream '{streamId}' to have version {expectedVersion} but is {root.Version}."
        //        );

        //    var document = BuildSnapShotDoc(snapshot, metadata, expectedVersion, streamId);

        //    await DbClient.UpsertDocumentAsync(DocCollectionUri,
        //        document,
        //        new RequestOptions { PartitionKey = key },
        //        true,
        //        cancellationToken
        //    );

        //    if (deleteOlderSnapshots)
        //        await DeleteSnapshotsAsync(streamId, expectedVersion, cancellationToken);
        //}

        //public async Task DeleteSnapshotsAsync(string streamId, long olderThanVersion,
        //    CancellationToken cancellationToken = default
        //)
        //{
        //    await ReadRootAsync(streamId, cancellationToken);

        //    var key = MakeKey(streamId);
        //    var feedOptions = new FeedOptions { PartitionKey = key };

        //    var query = MakeCamelCase(DbClient.CreateDocumentQuery<SnapshotDocument>(DocCollectionUri, feedOptions)
        //                .Where(x => x.StreamId == streamId)
        //                .Where(x => x.DocumentType == DocumentType.Snapshot)
        //                .Where(x => x.Version < olderThanVersion),
        //            DocCollectionUri,
        //            feedOptions
        //        )
        //        .AsDocumentQuery();

        //    while (query.HasMoreResults)
        //    {
        //        var page = await query.ExecuteNextAsync<Document>(cancellationToken);

        //        foreach (var document in page)
        //        {
        //            await DbClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, document.Id),
        //                new RequestOptions { PartitionKey = key },
        //                cancellationToken
        //            );
        //        }
        //    }
        //}


        //private async Task<int> BulkInsertEventsAsync(string streamId, List<DocumentBase> docs,
        //    CancellationToken cancellationToken = default
        //)
        //{
        //    var key = MakeKey(streamId);

        //    var ret = await DbClient.ExecuteStoredProcedureAsync<int>(
        //        UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, "bulkInsert"),
        //        new RequestOptions { EnableScriptLogging = true, PartitionKey = key },
        //        cancellationToken,
        //        nameof(streamId),
        //        streamId,
        //        docs
        //    );

        //    return ret;
        //}

        private static EventDocument BuildEventDoc(EventModel @event, string streamId, long streamVersion)
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

        //private static SnapshotDocument BuildSnapShotDoc(object snapshot, object metadata, long version, string streamId)
        //{
        //    var document = new SnapshotDocument
        //    {
        //        PartitionBy = streamId,
        //        StreamId = streamId,
        //        Version = version,
        //        SnapshotType = snapshot.GetType().AssemblyQualifiedName,
        //        SnapshotData = snapshot
        //    };

        //    if (metadata != null)
        //    {
        //        document.Metadata = metadata;
        //        document.MetadataType = metadata.GetType().AssemblyQualifiedName;
        //    }

        //    return document;
        //}

        public async Task<long> WriteToEventStreamAsync(string streamId, EventModel[] events, long? expectedVersion = null, object metadata = null,
            CancellationToken cancellationToken = default
        )
        {
            if (streamId.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException("A stream id is required.", nameof(streamId));

            if (events is null || events.Length == 0)
                throw new ArgumentException("You must supply events.", nameof(events));

            RootDocument root;
            var docs = new List<EventStoreDocument>();

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

            return root.Version;
        }

        public Task DeleteEventStreamAsync(string streamId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CreateSnapshotAsync(string streamId, long expectedVersion, object snapshot, object metadata = null, bool deleteOlderSnapshots = false,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }

        public Task DeleteSnapshotsAsync(string streamId, long olderThanVersion, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
