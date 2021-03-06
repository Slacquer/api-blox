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
using APIBlox.NetCore.Options;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace APIBlox.NetCore
{
    internal class CosmosDbRepository<TModel> : IEventStoreRepository<TModel>
        where TModel : class
    {
        private readonly IDocumentClient _client;
        private readonly ILogger _logger;
        private readonly CosmosDbOptions _options;
        private readonly JsonSerializerSettings _jsonSettings;

        public CosmosDbRepository(ILoggerFactory loggerFactory, IDocumentClient client, IEventStoreJsonSerializerSettings settings, IOptions<CosmosDbOptions> options)
        {
            _logger = loggerFactory.CreateLogger(GetType());

            _client = client ?? throw new ArgumentNullException(nameof(client));
            _options = options.Value;
            _jsonSettings = (JsonSerializerSettings) settings ?? new JsonSerializerSettings();;
        }

        public async Task<int> AddAsync<TDocument>(TDocument[] documents,
            CancellationToken cancellationToken = default
        )
            where TDocument : EventStoreDocument
        {
            await ExecuteAsync(async dbCol =>
                {
                    foreach (var doc in documents)
                    {
                        await _client.CreateDocumentAsync(dbCol.DocumentCollectionUri,
                            doc,
                            new RequestOptions
                            {
                                PartitionKey = new PartitionKey(doc.StreamId),
                                JsonSerializerSettings = _jsonSettings
                            },
                            true,
                            cancellationToken
                        );

                    }
                },
                cancellationToken: cancellationToken
            );

            return documents.Length;
        }

        public async Task<IEnumerable<TResultDocument>> GetAsync<TResultDocument>(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
            where TResultDocument : EventStoreDocument
        {
            var lst = new List<EventStoreDocument>();

            await ExecuteAsync(async dbCol =>
                {
                    var qry = _client.CreateDocumentQuery<EventStoreDocument>(dbCol.DocumentCollectionUri,
                            new FeedOptions
                            {
                                EnableCrossPartitionQuery = true,
                                JsonSerializerSettings = _jsonSettings
                            }
                        )
                        .Where(predicate)
                        .OrderByDescending(d => d.SortOrder)
                        .AsDocumentQuery();

                    while (qry.HasMoreResults)
                    {
                        var ret = await qry.ExecuteNextAsync<EventStoreDocument>(cancellationToken);

                        lst.AddRange(ret);
                    }
                },
                cancellationToken
            );

            return (IEnumerable<TResultDocument>)lst;
        }

        public async Task UpdateAsync<TDocument>(TDocument document,
            CancellationToken cancellationToken = default
        )
            where TDocument : EventStoreDocument
        {
            await ExecuteAsync(async dbCol =>
                {
                    await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(dbCol.DatabaseId, dbCol.CollectionId, document.StreamId),
                        document,
                        new RequestOptions
                        {
                            PartitionKey = new PartitionKey(document.StreamId),
                            JsonSerializerSettings = _jsonSettings
                        },
                        cancellationToken
                    );
                },
                cancellationToken
            );
        }

        public async Task<int> DeleteAsync(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
        {
            var count = 0;

            await ExecuteAsync(async dbCol =>
                {
                    var docs = await GetAsync<EventStoreDocument>(predicate, cancellationToken);

                    foreach (var doc in docs)
                    {
                        await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(dbCol.DatabaseId, dbCol.CollectionId, doc.Id),
                            new RequestOptions
                            {
                                PartitionKey = new PartitionKey(doc.StreamId),
                                JsonSerializerSettings = _jsonSettings
                            },
                            cancellationToken
                        );
                        count++;
                    }
                },
                cancellationToken

            );
           

            return count;
        }

        private async Task ExecuteAsync(Func<DbCollection, Task> cb,
            CancellationToken cancellationToken = default
        )
        {
            var dbCol = DbCollectionFactory.GetDatabaseAndCollection<TModel>(_options);

            while (true)
                try
                {
                    await cb(dbCol);

                    break;
                }
                catch (DocumentClientException dce)
                {
                    if (dce.StatusCode == HttpStatusCode.Conflict)
                        throw new EventStoreConcurrencyException($"A duplicated constraint (Unique Key) violation for {typeof(TModel).Name}.");

                    if (dce.StatusCode == HttpStatusCode.NotFound || dce.StatusCode == HttpStatusCode.Gone)
                    {
                        var createdDb = await _client.CreateDbColDatabaseAsync(_logger, dbCol, cancellationToken);
                        var createdCol = await _client.CreateDbColCollectionAsync(_logger, dbCol, cancellationToken);

                        if (createdDb || createdCol)
                            continue;

                        throw new EventStoreNotFoundException("Execution failure", dce);
                    }

                    if (dce.StatusCode == HttpStatusCode.ServiceUnavailable)

                        // Hmmm?  this happens quite a bit locally.
                        throw new EventStoreAccessException(
                            "Cosmos is DOWN.  Execution failure",
                            dce
                        );

                    throw;
                }
        }
    }
}
