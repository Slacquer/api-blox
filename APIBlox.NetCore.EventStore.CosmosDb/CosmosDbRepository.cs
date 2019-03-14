using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Exceptions;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIBlox.NetCore.EventStore.CosmosDb
{
    internal class CosmosDbRepository<TModel> : IEventStoreRepository
    {
        private readonly IDocumentClient _client;
        private readonly string _collectionId;
        private readonly string _databaseId;
        private readonly Uri _docCollectionUri;
        private readonly List<string> _uniqueKeys;

        public CosmosDbRepository(IDocumentClient client, JsonSerializerSettings settings, IOptions<CosmosDbOptions> options)
        {
            var opt = options.Value;
            var col = opt.Collections.First(c => c.Key.EqualsEx(typeof(TModel).Name)).Value;

            _uniqueKeys = col.UniqueKeys.ToList();

            _collectionId = col.Id ?? throw new ArgumentNullException(nameof(col.Id));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _databaseId = opt.DatabaseId ?? throw new ArgumentNullException(nameof(opt.DatabaseId));
            _docCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId);
            JsonSettings = settings ?? throw new ArgumentNullException(nameof(settings));

            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        public JsonSerializerSettings JsonSettings { get; set; }

        public async Task<int> AddAsync<TDocument>(TDocument[] documents,
            CancellationToken cancellationToken = default
        )
            where TDocument : EventStoreDocument
        {
            foreach (var doc in documents)
            {
                await _client.CreateDocumentAsync(_docCollectionUri,
                    doc,
                    new RequestOptions
                    {
                        PartitionKey = new PartitionKey(doc.StreamId)
                    },
                    true,
                    cancellationToken
                );
            }

            return documents.Length;
        }

        public async Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
            where TResult : class
        {
            var isString = typeof(TResult) == typeof(string);

            try
            {
                var qry = _client.CreateDocumentQuery<EventStoreDocument>(_docCollectionUri,
                        new FeedOptions {EnableCrossPartitionQuery = true}
                    )
                    .Where(predicate)
                    .OrderByDescending(d => d.SortOrder)
                    .AsDocumentQuery();

                var lst = new List<TResult>();

                while (qry.HasMoreResults)
                {
                    var ret = await qry.ExecuteNextAsync<Document>(cancellationToken);

                    foreach (var document in ret)
                    {
                        var result = isString
                            ? document.ToString() as TResult
                            : JsonConvert.DeserializeObject<TResult>(JsonConvert.SerializeObject(document, JsonSettings), JsonSettings);

                        lst.Add(result);
                    }
                }

                return lst;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.Conflict)
                    throw new DocumentConcurrencyException(e.Message);

                throw;
            }
        }

        public async Task UpdateAsync<TDocument>(TDocument document,
            CancellationToken cancellationToken = default
        )
            where TDocument : EventStoreDocument
        {
            try
            {
                await _client.ReplaceDocumentAsync(RootDocumentUri(document.StreamId),
                    document,
                    new RequestOptions
                    {
                        PartitionKey = new PartitionKey(document.StreamId)
                    },
                    cancellationToken
                );
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                    throw new DocumentNotFoundException($"Document with stream id '{document.StreamId}' not found!");

                throw;
            }
        }

        public async Task<int> DeleteAsync(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
        {
            var count = 0;

            try
            {
                var docs = await GetAsync<EventStoreDocument>(predicate, cancellationToken);

                foreach (var doc in docs)
                {
                    await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, doc.Id),
                        new RequestOptions
                        {
                            PartitionKey = new PartitionKey(doc.StreamId)
                        },
                        cancellationToken
                    );
                    count++;
                }
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                    throw new DocumentNotFoundException(e.Message);

                throw;
            }

            return count;
        }

        //private void SetJsonSettings(IDocumentClient client)
        //{
        //    if (!(JsonSettings is null))
        //        return;

        //    var tmp = new CamelCaseSettings();
        //    tmp.Converters.Add(new StringEnumConverter());

        //    JsonSettings = (JsonSerializerSettings) client.GetType().GetField("serializerSettings",
        //                       BindingFlags.GetField
        //                       | BindingFlags.Instance | BindingFlags.NonPublic
        //                   ).GetValue(client)
        //                   ?? tmp;
        //}

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            var exists = await _client.CreateDatabaseQuery()
                .Where(d => d.Id == _databaseId)
                .ToAsyncEnumerable().Any();

            if (exists)
                return;

            await _client.CreateDatabaseAsync(new Database {Id = _databaseId});
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            var exists = await _client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(_databaseId))
                .Where(d => d.Id == _collectionId)
                .ToAsyncEnumerable().Any();

            if (exists)
                return;

            var documentCollection = new DocumentCollection
            {
                Id = _collectionId
            };
            documentCollection.PartitionKey.Paths.Add("/streamId");

            if (_uniqueKeys.Any())
            {
                var uniqueKey = new UniqueKey();

                foreach (var key in _uniqueKeys)
                    uniqueKey.Paths.Add(key);

                documentCollection.UniqueKeyPolicy.UniqueKeys.Add(uniqueKey);
            }

            await _client.CreateDocumentCollectionAsync(
                UriFactory.CreateDatabaseUri(_databaseId),
                documentCollection,
                new RequestOptions {OfferThroughput = 1000}
            );
        }

        private Uri RootDocumentUri(string streamId)
        {
            return UriFactory.CreateDocumentUri(_databaseId, _collectionId, RootDocument.GenerateId(streamId));
        }

        //protected IQueryable<T> MakeCamelCase<T>(IQueryable<T> query, Uri collUri, FeedOptions opts)
        //{
        //    if (query.Expression.NodeType == System.Linq.Expressions.ExpressionType.Constant)
        //        return query;

        //    dynamic sqlQueryObject = JsonConvert.DeserializeObject(query.ToString());
        //    string sql = sqlQueryObject.query;
        //    // Regex is actually a private member w/ Compiled flag set to avoid overhead.  Not that it really matters,
        //    // the guts of MS.DocumentDB.Core use uncached reflection with abandon
        //    sql = new Regex("\\[\"(.+?)\"\\]").Replace(sql, match => $"[\"{match.Groups[1].Value.ToCamelCase()}\"]");
        //    query = DbClient.CreateDocumentQuery<T>(collUri, sql, opts);

        //    return query;
        //}

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
