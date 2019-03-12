using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Options;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace APIBlox.NetCore
{
    internal class CosmosDbRepository<TModel> : IEventStoreRepository<EventStoreDocument>
    {
        private readonly string _bulkInsertFilePath;
        private readonly List<string> _uniqueKeys;

        protected IDocumentClient DbClient { get; }

        protected string CollectionId { get; }

        protected string DatabaseId { get; }

        protected Uri DocCollectionUri { get; }

        public JsonSerializerSettings JsonSettings { get; set; }

        public CosmosDbRepository(IDocumentClient client, IOptions<CosmosDbOptions> options)
        {
            var opt = options.Value;
            var col = opt.Collections.First(c => c.Key.EqualsEx(typeof(TModel).Name)).Value;

            _uniqueKeys = col.UniqueKeys.ToList();

            CollectionId = col.Id ?? throw new ArgumentNullException(nameof(col.Id));
            DbClient = client ?? throw new ArgumentNullException(nameof(client));
            DatabaseId = opt.DatabaseId ?? throw new ArgumentNullException(nameof(opt.DatabaseId));
            DocCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);

            JsonSettings = new CamelCaseSettings();

            _bulkInsertFilePath = opt.BulkInsertFilePath;

            if (!File.Exists(_bulkInsertFilePath))
                throw new ArgumentException("Bulk insert file does not exist!", nameof(opt.BulkInsertFilePath));

            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
            CreateBulkInsertSprocIfNotExistsAsync().Wait();
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


        protected Uri RootDocumentUri(string streamId)
        {
            return UriFactory.CreateDocumentUri(DatabaseId, CollectionId, RootDocument.GenerateId(streamId));
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


        private FeedOptions MakeFeedOptions(string id)
        {
            return new FeedOptions { PartitionKey = new PartitionKey(id) };
        }

        public async Task<int> AddAsync(CancellationToken cancellationToken = default, params EventStoreDocument[] documents)
        {
            foreach (var doc in documents)
            {
                await DbClient.CreateDocumentAsync(DocCollectionUri, doc, new RequestOptions
                {
                    PartitionKey = new PartitionKey(doc.StreamId)
                }, true, cancellationToken);
            }

            return documents.Length;
        }

        public async Task<EventStoreDocument> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var qry = DbClient.CreateDocumentQuery<EventStoreDocument>(DocCollectionUri)//, MakeFeedOptions(id))
                .Where(d => d.Id == id).AsDocumentQuery();

            while (qry.HasMoreResults)
            {
                var ret = await qry.ExecuteNextAsync<EventStoreDocument>(cancellationToken);

                return ret.First();
            }

            return null;
        }

        public async Task<EventStoreDocument> GetByStreamIdAsync(string streamId, CancellationToken cancellationToken = default)
        {
            var qry = DbClient.CreateDocumentQuery<EventStoreDocument>(DocCollectionUri, MakeFeedOptions(streamId))
                .Where(d => d.Id == streamId).AsDocumentQuery();

            while (qry.HasMoreResults)
            {
                var ret = await qry.ExecuteNextAsync(cancellationToken);

                return ret.First();
            }

            return null;
        }

        public Task<IEnumerable<EventStoreDocument>> GetAsync(Func<EventStoreDocument, bool> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(EventStoreDocument eventObject, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Func<EventStoreDocument, bool> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
