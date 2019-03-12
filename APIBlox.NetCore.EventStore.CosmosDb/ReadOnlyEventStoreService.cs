#region -    Using Statements    -

using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IEventStoreRepository<DocumentBase> _repo;

        #region -    Constructors    -

        public ReadOnlyEventStoreService(IEventStoreRepository<DocumentBase> repo)
        {
            _repo = repo;
        }

        #endregion

        

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

        //private static EventModel Deserialize(EventDocument document)
        //{
        //    object metadata = null;

        //    if (!string.IsNullOrEmpty(document.MetadataType))
        //        metadata = document.Metadata;

        //    object body = null;

        //    if (!string.IsNullOrEmpty(document.EventType))
        //        body = document.EventData;

        //    return new EventModel(body, document.Version, document.TimeStamp, metadata);
        //}

        //private static SnapshotModel Deserialize(SnapshotDocument document)
        //{
        //    object metadata = null;

        //    if (!string.IsNullOrEmpty(document.MetadataType))
        //        metadata = document.Metadata;

        //    return new SnapshotModel(document.SnapshotData,
        //        metadata,
        //        document.Version,
        //        document.TimeStamp
        //    );
        //}

        public JsonSerializerSettings JsonSettings { get; set; }

        public Task<EventStreamModel> ReadEventStreamAsync(string streamId, long? fromVersion = null, bool includeEvents = false, Func<object> initializeSnapshotObject = null,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }
    }
}
