﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Exceptions;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Options;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIBlox.NetCore
{
    internal class CosmosDbRepository<TModel> : IEventStoreRepository
    {
        private readonly List<string> _uniqueKeys;
        private readonly IDocumentClient _client;
        private readonly string _collectionId;
        private readonly string _databaseId;
        private readonly Uri _docCollectionUri;

        public CosmosDbRepository(IDocumentClient client, IOptions<CosmosDbOptions> options)
        {
            var opt = options.Value;
            var col = opt.Collections.First(c => c.Key.EqualsEx(typeof(TModel).Name)).Value;

            _uniqueKeys = col.UniqueKeys.ToList();

            _collectionId = col.Id ?? throw new ArgumentNullException(nameof(col.Id));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _databaseId = opt.DatabaseId ?? throw new ArgumentNullException(nameof(opt.DatabaseId));
            _docCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId);

            JsonSettings = new CamelCaseSettings();

            JsonSettings.Converters.Add(new StringEnumConverter());

            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        public JsonSerializerSettings JsonSettings { get; set; }

        public async Task<int> AddAsync<TDocument>(TDocument[] documents, 
            CancellationToken cancellationToken = default)
            where TDocument : IEventStoreDocument
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

        public async Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<IEventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var qry = _client.CreateDocumentQuery<IEventStoreDocument>(_docCollectionUri,
                        new FeedOptions {EnableCrossPartitionQuery = true}
                    )
                    .Where(predicate)
                    .OrderByDescending(d => d.SortOrder)
                    .AsDocumentQuery();

                var lst = new List<TResult>();

                while (qry.HasMoreResults)
                {
                    var ret = await qry.ExecuteNextAsync<TResult>(cancellationToken);
                    lst.AddRange(ret.ToList());
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
            CancellationToken cancellationToken = default)
            where TDocument : IEventStoreDocument
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

        public async Task<int> DeleteAsync(Expression<Func<IEventStoreDocument, bool>> predicate, 
            CancellationToken cancellationToken = default)
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
