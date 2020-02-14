using System;
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
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Index = Microsoft.Azure.Documents.Index;

namespace APIBlox.NetCore
{
    internal class CosmosDbRepository<TModel> : IEventStoreRepository<TModel>
        where TModel : class
    {
        private readonly IDocumentClient _client;
        private readonly string _collectionId;
        private readonly string _databaseId;
        private readonly Uri _docCollectionUri;

        public CosmosDbRepository(IDocumentClient client, JsonSerializerSettings settings, IOptions<CosmosDbOptions> options)
        {
            var opt = options.Value;
            var col = opt.CollectionProperties.FirstOrDefault(c => c.Key.EqualsEx(typeof(TModel).Name)).Value;

            var colValue = col ?? throw new ArgumentNullException(nameof(IOptions<CosmosDbOptions>),
                               $"CollectionProperty value for '{typeof(TModel).Name}' was not found!"
                           );

            _collectionId = col.CollectionName ?? throw new ArgumentNullException(nameof(col.CollectionName));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _databaseId = opt.DatabaseId ?? throw new ArgumentNullException(nameof(opt.DatabaseId));
            _docCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId);
            JsonSettings = settings ?? throw new ArgumentNullException(nameof(settings));

            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync(colValue.UniqueKeys.ToList(), colValue.OfferThroughput).Wait();
        }

        public JsonSerializerSettings JsonSettings { get; }

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

        public async Task<IEnumerable<TResultDocument>> GetAsync<TResultDocument>(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
            where TResultDocument : EventStoreDocument
        {
            var qry = _client.CreateDocumentQuery<EventStoreDocument>(_docCollectionUri,
                    new FeedOptions { EnableCrossPartitionQuery = true }
                )
                .Where(predicate)
                .OrderByDescending(d => d.SortOrder)
                .AsDocumentQuery();

            var lst = new List<EventStoreDocument>();

            while (qry.HasMoreResults)
            {
                var ret = await qry.ExecuteNextAsync<EventStoreDocument>(cancellationToken);

                foreach (var document in ret)
                    lst.Add(document);
            }

            return (IEnumerable<TResultDocument>)lst;
        }

        public async Task UpdateAsync<TDocument>(TDocument document,
            CancellationToken cancellationToken = default
        )
            where TDocument : EventStoreDocument
        {
            try
            {
                await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, document.StreamId),
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
                    throw new EventStoreNotFoundException($"Document with stream id '{document.StreamId}' not found!");

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
                    throw new EventStoreNotFoundException(e.Message);

                throw;
            }

            return count;
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            var exists = await _client.CreateDatabaseQuery()
                .Where(d => d.Id == _databaseId)
                .ToAsyncEnumerable().AnyAsync();

            if (exists)
                return;

            await _client.CreateDatabaseAsync(new Database { Id = _databaseId });
        }

        private async Task CreateCollectionIfNotExistsAsync(IReadOnlyCollection<string> keys, int offerThroughput)
        {
            var exists = await _client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(_databaseId))
                .Where(d => d.Id == _collectionId)
                .ToAsyncEnumerable().AnyAsync();

            if (exists)
                return;

            var documentCollection = new DocumentCollection
            {
                Id = _collectionId
            };
            documentCollection.PartitionKey.Paths.Add("/StreamId");

            var p = new IncludedPath { Path = "/" };
            var rng = Index.Range(DataType.String);

            documentCollection.IndexingPolicy.IncludedPaths.Add(p);

            p = new IncludedPath { Path = "/StreamId/?" };
            p.Indexes.Add(rng);
            documentCollection.IndexingPolicy.IncludedPaths.Add(p);

            p = new IncludedPath { Path = "/DocumentType/?" };
            p.Indexes.Add(rng);
            documentCollection.IndexingPolicy.IncludedPaths.Add(p);

            if (keys.Any())
            {
                var uniqueKey = new UniqueKey();

                foreach (var key in keys)
                    uniqueKey.Paths.Add(key);

                documentCollection.UniqueKeyPolicy.UniqueKeys.Add(uniqueKey);
            }

            await _client.CreateDocumentCollectionAsync(
                UriFactory.CreateDatabaseUri(_databaseId),
                documentCollection,
                new RequestOptions { OfferThroughput = offerThroughput }
            );
        }
    }
}
