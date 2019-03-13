#region -    Using Statements    -

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Exceptions;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Models;
using APIBlox.NetCore.Options;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

#endregion

namespace APIBlox.NetCore
{
    [InjectableService(ServiceLifetime = ServiceLifetime.Singleton)]
    internal class ReadOnlyEventStoreService<TModel> : IReadOnlyEventStoreService<TModel>
        where TModel : class, IEventStoreDocument
    {
        protected readonly IEventStoreRepository Repository;

        #region -    Constructors    -

        public ReadOnlyEventStoreService(IEventStoreRepository repo)
        {
            Repository = repo;
        }

        #endregion

        protected async Task<RootDocument> ReadRootAsync(string streamId, CancellationToken cancellationToken = default)
        {
            var ret = (await Repository.GetAsync<Document>(
                d => d.StreamId == streamId && d.DocumentType == DocumentType.Root,
                cancellationToken
            )).FirstOrDefault();

            if (ret is null)
                throw new DataAccessException($"Stream '{streamId}' wasn't found");

            return (RootDocument)EventStoreDocument.Parse(ret, null, Repository.JsonSettings);
        }

        //public async Task<EventStreamModel> ReadEventStreamAsync(string streamId,
        //    long? fromVersion = null,
        //    bool includeEvents = false,
        //    Func<object> initializeSnapshotObject = null,
        //    CancellationToken cancellationToken = default
        //)
        //{
        //    if (streamId == null)
        //        throw new ArgumentNullException(nameof(streamId));

        //    var key = MakeKey(streamId);
        //    var feedOptions = new FeedOptions
        //    {
        //        PartitionKey = key,
        //        EnableCrossPartitionQuery = true
        //    };

        //    if (fromVersion.HasValue && fromVersion > 0)
        //        await VersionCheckAsync(streamId, fromVersion, cancellationToken, feedOptions);

        //    var query = DbClient.CreateDocumentQuery<DocumentBase>(DocCollectionUri, feedOptions)
        //        .Where(d => d.StreamId == streamId);

        //    if (fromVersion.HasValue)
        //        query = query.Where(d => d.Version >= fromVersion);

        //    var qry = MakeCamelCase(query.OrderByDescending(d => d.SortOrder), DocCollectionUri, feedOptions).AsDocumentQuery();

        //    var documents = new List<DocumentBase>();
        //    var finishLoading = false;

        //    do
        //    {
        //        var page = await qry.ExecuteNextAsync<Document>(cancellationToken);

        //        foreach (var document in page)
        //        {
        //            var doc = DocumentBase.Parse(document, initializeSnapshotObject, JsonSettings);

        //            documents.Add(doc);

        //            if (doc is RootDocument && !includeEvents)
        //            {
        //                finishLoading = true;
        //                break;
        //            }

        //            if (!(doc is SnapshotDocument) || !(fromVersion is null))
        //                continue;

        //            finishLoading = true;
        //            break;
        //        }

        //        if (finishLoading)
        //            break;
        //    } while (qry.HasMoreResults);

        //    if (documents.Count == 0)
        //        return null;

        //    if (!(documents.First() is RootDocument rootDocument))
        //        return null;

        //    object metadata = null;

        //    if (!string.IsNullOrEmpty(rootDocument.MetadataType))
        //        metadata = rootDocument.Metadata;

        //    var events = documents.OfType<EventDocument>()
        //        .Select(Deserialize).Reverse()
        //        .ToArray();

        //    var snapshot = fromVersion is null ? documents.OfType<SnapshotDocument>().Select(Deserialize).FirstOrDefault() : null;

        //    return new EventStreamModel(streamId, rootDocument.Version, rootDocument.TimeStamp, metadata, events, snapshot);
        //}

        //private async Task VersionCheckAsync(string streamId,
        //    long? expectedVersion, CancellationToken cancellationToken, FeedOptions feedOptions
        //)
        //{
        //    var root = await ReadRootAsync(streamId, cancellationToken);

        //    // if for whatever reason, the incoming version is greater than what is stored then something is a miss...
        //    if (root.Version < expectedVersion)
        //        throw new DataConcurrencyException(
        //            $"Provided version:{expectedVersion} for stream '{streamId}' is greater than the event source version:{root.Version}!"
        //        );

        //    // we must also make sure that the provided version actually exists!
        //    var exists = (await MakeCamelCase(DbClient.CreateDocumentQuery<DocumentBase>(DocCollectionUri, feedOptions)
        //        .Where(d => d.Version == expectedVersion), DocCollectionUri, feedOptions)
        //        .AsDocumentQuery().ExecuteNextAsync(cancellationToken)).Any();

        //    if (!exists)
        //        throw new DataConcurrencyException($"Provided version:{expectedVersion} for stream '{streamId}' does not exist!");
        //}



        //protected async Task<RootDocument> ReadRootAsync(string streamId, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        var key = MakeKey(streamId);

        //        return await DbClient.ReadDocumentAsync<RootDocument>(RootDocumentUri(streamId),
        //            new RequestOptions
        //            {
        //                PartitionKey = key
        //            },
        //            cancellationToken
        //        );
        //    }
        //    catch (DocumentClientException ex) when (ex.Error.Code == nameof(HttpStatusCode.NotFound))
        //    {
        //        throw new DataAccessException($"Stream '{streamId}' wasn't found");
        //    }
        //}

        //protected async Task<IReadOnlyCollection<PartitionKeyRange>> GetPartitionKeyRanges()
        //{
        //    string responseContinuation = null;
        //    var partitionKeyRanges = new List<PartitionKeyRange>();

        //    do
        //    {
        //        var response = await DbClient.ReadPartitionKeyRangeFeedAsync(DocCollectionUri,
        //            new FeedOptions { RequestContinuation = responseContinuation }
        //        );

        //        partitionKeyRanges.AddRange(response);
        //        responseContinuation = response.ResponseContinuation;
        //    } while (responseContinuation != null);

        //    return partitionKeyRanges;
        //}

        //protected static PartitionKey MakeKey(string partitionedByValue)
        //{
        //    return new PartitionKey(partitionedByValue);
        //}

        private static EventModel Deserialize(EventDocument document)
        {
            object metadata = null;

            if (!string.IsNullOrEmpty(document.MetadataType))
                metadata = document.Metadata;

            object body = null;

            if (!string.IsNullOrEmpty(document.EventType))
                body = document.EventData;

            return new EventModel { Data = body, Version = document.Version, TimeStamp = document.TimeStamp, Metadata = metadata };
        }

        private static SnapshotModel Deserialize(SnapshotDocument document)
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

        public async Task<EventStreamModel> ReadEventStreamAsync(string streamId, long? fromVersion = null, bool includeEvents = false, Func<object> initializeSnapshotObject = null,
            CancellationToken cancellationToken = default
        )
        {
            if (streamId == null)
                throw new ArgumentNullException(nameof(streamId));

            if (fromVersion.HasValue && fromVersion > 0)
                await VersionCheckAsync(streamId, fromVersion, cancellationToken);

            Expression<Func<IEventStoreDocument, bool>> predicate = e =>
                fromVersion.HasValue ? e.StreamId == streamId && e.Version >= fromVersion : e.StreamId == streamId;

            var results = (await Repository.GetAsync<Document>(predicate, cancellationToken)).ToList();

            if (results.Count == 0)
                return null;

            var docs = new List<EventStoreDocument>();

            foreach (var document in results)
            {
                var doc = EventStoreDocument.Parse(document, initializeSnapshotObject, Repository.JsonSettings);

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
                .Select(Deserialize).Reverse()
                .ToArray();

            var snapshot = fromVersion is null ? docs.OfType<SnapshotDocument>().Select(Deserialize).FirstOrDefault() : null;

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

        private async Task VersionCheckAsync(string streamId,
            long? expectedVersion, CancellationToken cancellationToken
        )
        {
            var root = await ReadRootAsync(streamId, cancellationToken);

            // if for whatever reason, the incoming version is greater than what is stored then something is a miss...
            if (root.Version < expectedVersion)
                throw new DocumentConcurrencyException(
                    $"Provided version:{expectedVersion} for stream '{streamId}' is greater than the event source version:{root.Version}!"
                );
        }
    }
}
