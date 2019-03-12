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
    internal class ReadOnlyEventStoreService<TAggregate> : IReadOnlyEventStoreService<TAggregate>
        where TAggregate : class
    {

        private readonly List<string> _uniqueKeys;


        #region -    Constructors    -

        public ReadOnlyEventStoreService(IDocumentClient client, IOptions<EventStoreOptions> options)
        {
            var opt = options.Value;
            var col = opt.Collections.First(c => c.Key.EqualsEx(typeof(TAggregate).Name)).Value;
            
            _uniqueKeys = col.UniqueKeys.ToList();

            CollectionId = col.Id ?? throw new ArgumentNullException(nameof(col.Id));
            DbClient = client ?? throw new ArgumentNullException(nameof(client));
            DatabaseId = opt.DatabaseId ?? throw new ArgumentNullException(nameof(opt.DatabaseId));
            DocCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);

            JsonSettings =
                (JsonSerializerSettings)client.GetType()
                    .GetField("serializerSettings", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic).GetValue(DbClient)
                ?? new CamelCaseSettings();

            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        #endregion

        protected IDocumentClient DbClient { get; }
        protected string CollectionId { get; }
        protected string DatabaseId { get; }
        protected Uri DocCollectionUri { get; }

        public JsonSerializerSettings JsonSettings { get; set; }

        public async Task<EventStreamModel> ReadEventStreamAsync(string streamId,
            ulong? fromVersion = null,
            bool includeEvents = false,
            Func<object> initializeSnapshotObject = null,
            CancellationToken cancellationToken = default
        )
        {
            if (streamId == null)
                throw new ArgumentNullException(nameof(streamId));

            var key = MakeKey(streamId);
            var feedOptions = new FeedOptions
            {
                PartitionKey = key,
                EnableCrossPartitionQuery = true
            };

            if (fromVersion.HasValue && fromVersion > 0)
                await VersionCheckAsync(streamId, fromVersion, cancellationToken, feedOptions);

            var query = DbClient.CreateDocumentQuery<DocumentBase>(DocCollectionUri, feedOptions)
                .Where(d => d.StreamId == streamId);

            if (fromVersion.HasValue)
                query = query.Where(d => d.Version >= fromVersion);

            var qry = MakeCamelCase(query.OrderByDescending(d => d.SortOrder), DocCollectionUri, feedOptions).AsDocumentQuery();

            var documents = new List<DocumentBase>();
            var finishLoading = false;

            do
            {
                var page = await qry.ExecuteNextAsync<Document>(cancellationToken);

                foreach (var document in page)
                {
                    var doc = DocumentBase.Parse(document, initializeSnapshotObject, JsonSettings);

                    documents.Add(doc);

                    if (doc is RootDocument && !includeEvents)
                    {
                        finishLoading = true;
                        break;
                    }

                    if (!(doc is SnapshotDocument) || !(fromVersion is null))
                        continue;

                    finishLoading = true;
                    break;
                }

                if (finishLoading)
                    break;
            } while (qry.HasMoreResults);

            if (documents.Count == 0)
                return null;

            if (!(documents.First() is RootDocument rootDocument))
                return null;

            object metadata = null;

            if (!string.IsNullOrEmpty(rootDocument.MetadataType))
                metadata = rootDocument.Metadata;

            var events = documents.OfType<EventDocument>()
                .Select(Deserialize).Reverse()
                .ToArray();

            var snapshot = fromVersion is null ? documents.OfType<SnapshotDocument>().Select(Deserialize).FirstOrDefault() : null;

            return new EventStreamModel(streamId, rootDocument.Version, rootDocument.TimeStamp, metadata, events, snapshot);
        }

        //public async Task<EventStreamModel> GetStreamRootAsync(string streamId, string partitionedByValue, CancellationToken cancellationToken = default)
        //{
        //    if (streamId == null)
        //        throw new ArgumentNullException(nameof(streamId));

        //    var key = MakeKey(partitionedByValue);
        //    var feedOptions = new FeedOptions
        //    {
        //        PartitionKey = key,
        //        EnableCrossPartitionQuery = true
        //    };

        //    var existing = (await MakeCamelCase(DbClient.CreateDocumentQuery<RootDocument>(DocCollectionUri, feedOptions)
        //        .Where(d => d.StreamId == streamId && d.DocumentType == DocumentType.Root), DocCollectionUri, feedOptions)
        //        .AsDocumentQuery()
        //        .ExecuteNextAsync<RootDocument>(cancellationToken)).FirstOrDefault();

        //    return existing is null ? null : new EventStreamModel(streamId, existing.Version, existing.TimeStamp, existing.Metadata);
        //}

        //public async Task<IEnumerable<string>> GetAllPartitionKeyValuesAsync(CancellationToken cancellationToken = default)
        //{
        //    var feedOptions = new FeedOptions
        //    {
        //        EnableCrossPartitionQuery = true
        //    };

        //    var ret = new List<string>();

        //    var qry = DbClient.CreateDocumentQuery<RootDocument>(DocCollectionUri, new SqlQuerySpec("SELECT DISTINCT c.partitionBy FROM c WHERE c.documentType='Root'"), feedOptions)
        //        .AsDocumentQuery();

        //    while (qry.HasMoreResults)
        //    {
        //        foreach (var p in await qry.ExecuteNextAsync<RootDocument>(cancellationToken))
        //            ret.Add(p.PartitionBy);
        //    }

        //    return ret.OrderBy(s => s);
        //}

        private async Task VersionCheckAsync(string streamId,
            ulong? expectedVersion, CancellationToken cancellationToken, FeedOptions feedOptions
        )
        {
            var root = await ReadRootAsync(streamId, cancellationToken);

            // if for whatever reason, the incoming version is greater than what is stored then something is a miss...
            if (root.Version < expectedVersion)
                throw new DataConcurrencyException(
                    $"Provided version:{expectedVersion} for stream '{streamId}' is greater than the event source version:{root.Version}!"
                );

            // we must also make sure that the provided version actually exists!
            var exists = (await MakeCamelCase(DbClient.CreateDocumentQuery<DocumentBase>(DocCollectionUri, feedOptions)
                .Where(d => d.Version == expectedVersion), DocCollectionUri, feedOptions)
                .AsDocumentQuery().ExecuteNextAsync(cancellationToken)).Any();

            if (!exists)
                throw new DataConcurrencyException($"Provided version:{expectedVersion} for stream '{streamId}' does not exist!");
        }

        protected Uri RootDocumentUri(string streamId)
        {
            return UriFactory.CreateDocumentUri(DatabaseId, CollectionId, RootDocument.GenerateId(streamId));
        }

        protected async Task<RootDocument> ReadRootAsync(string streamId,  CancellationToken cancellationToken = default)
        {
            try
            {
                var key = MakeKey(streamId);

                return await DbClient.ReadDocumentAsync<RootDocument>(RootDocumentUri(streamId),
                    new RequestOptions
                    {
                        PartitionKey = key
                    },
                    cancellationToken
                );
            }
            catch (DocumentClientException ex) when (ex.Error.Code == nameof(HttpStatusCode.NotFound))
            {
                throw new DataAccessException($"Stream '{streamId}' wasn't found");
            }
        }

        protected async Task<IReadOnlyCollection<PartitionKeyRange>> GetPartitionKeyRanges()
        {
            string responseContinuation = null;
            var partitionKeyRanges = new List<PartitionKeyRange>();

            do
            {
                var response = await DbClient.ReadPartitionKeyRangeFeedAsync(DocCollectionUri,
                    new FeedOptions { RequestContinuation = responseContinuation }
                );

                partitionKeyRanges.AddRange(response);
                responseContinuation = response.ResponseContinuation;
            } while (responseContinuation != null);

            return partitionKeyRanges;
        }

        protected static PartitionKey MakeKey(string partitionedByValue)
        {
            return new PartitionKey(partitionedByValue);
        }

        private static EventModel Deserialize(EventDocument document)
        {
            object metadata = null;

            if (!string.IsNullOrEmpty(document.MetadataType))
                metadata = document.Metadata;

            object body = null;

            if (!string.IsNullOrEmpty(document.EventType))
                body = document.EventData;

            return new EventModel(body, document.Version, document.TimeStamp, metadata);
        }

        private static SnapshotModel Deserialize(SnapshotDocument document)
        {
            object metadata = null;

            if (!string.IsNullOrEmpty(document.MetadataType))
                metadata = document.Metadata;

            return new SnapshotModel(document.SnapshotData,
                metadata,
                document.Version,
                document.TimeStamp
            );
        }

        //
        // In a nutshell, apparently the linq provider does not honor camelCase serialization.
        // So rather than having to put json attributes on all properties we use this for now...
        //
        // See https://github.com/Azure/azure-cosmosdb-dotnet/issues/317 ... this code is a ridiculously brittle kludge that works for now
        protected IQueryable<T> MakeCamelCase<T>(IQueryable<T> query, Uri collUri, FeedOptions opts)
        {
            if (query.Expression.NodeType == System.Linq.Expressions.ExpressionType.Constant)
                return query;

            dynamic sqlQueryObject = JsonConvert.DeserializeObject(query.ToString());
            string sql = sqlQueryObject.query;
            // Regex is actually a private member w/ Compiled flag set to avoid overhead.  Not that it really matters,
            // the guts of MS.DocumentDB.Core use uncached reflection with abandon
            sql = new Regex("\\[\"(.+?)\"\\]").Replace(sql, match => $"[\"{match.Groups[1].Value.ToCamelCase()}\"]");
            query = DbClient.CreateDocumentQuery<T>(collUri, sql, opts);

            return query;
        }


        private async Task CreateDatabaseIfNotExistsAsync()
        {
            var exists = await DbClient.CreateDatabaseQuery()
                .Where(d => d.Id == DatabaseId)
                .ToAsyncEnumerable().Any();

            if (exists)
                return;

            await DbClient.CreateDatabaseAsync(new Database { Id = DatabaseId });
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            var exists = await DbClient.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(DatabaseId))
                .Where(d => d.Id == CollectionId)
                .ToAsyncEnumerable().Any();

            if (exists)
                return;

            var documentCollection = new DocumentCollection
            {
                Id = CollectionId
            };
            documentCollection.PartitionKey.Paths.Add("/partitionBy");

            if (_uniqueKeys.Any())
            {
                var uniqueKey = new UniqueKey();

                foreach (var key in _uniqueKeys)
                    uniqueKey.Paths.Add(key);

                documentCollection.UniqueKeyPolicy.UniqueKeys.Add(uniqueKey);
            }

            await DbClient.CreateDocumentCollectionAsync(
                UriFactory.CreateDatabaseUri(DatabaseId),
                documentCollection,
                new RequestOptions { OfferThroughput = 1000 }
            );
        }

        //public Task RetrievePartitionsEvents(Func<IReadOnlyCollection<EventModel>, Task> callback, string partitionValue,
        //    CancellationToken cancellationToken = default
        //)
        //{
        //    return LoadChangeFeed(documents => callback(documents.OfType<EventDocument>().Select(Deserialize).ToList()),
        //        partitionValue,
        //        cancellationToken
        //    );
        //}

        //private async Task LoadChangeFeed(Func<IEnumerable<DocumentBase>, Task> callback,
        //    string partitionedByValue,
        //    CancellationToken cancellationToken = default
        //)
        //{
        //    var key = MakeKey(partitionedByValue);
        //    var changeFeed = _client.CreateDocumentChangeFeedQuery(_docCollectionUri,
        //        new ChangeFeedOptions
        //        {
        //            // PartitionKeyRangeId = pkRange.Id,
        //            PartitionKey = key,

        //            // RequestContinuation = token,
        //            StartFromBeginning = true,
        //            MaxItemCount = -1
        //        }
        //    );

        //    Task callbackTask = null;

        //    while (changeFeed.HasMoreResults)
        //    {
        //        var page = await changeFeed.ExecuteNextAsync<Document>(cancellationToken);

        //        if (callbackTask != null)
        //            await callbackTask;

        //        callbackTask = callback(page.Select(x => DocumentBase.Parse(x, _jsonSerializerSettings)));
        //    }
        //}
    }
}
