using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Options;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace APIBlox.NetCore
{
    internal static class DbCollectionFactory
    {
        private static readonly object DbColLock = new object();
        private static readonly Dictionary<Type, DbCollection> DbCols = new Dictionary<Type, DbCollection>();

        public static DbCollection GetDatabaseAndCollection<TModel>(CosmosDbOptions options)
        {
            lock (DbColLock)
            {
                var t = typeof(TModel);

                var existing = DbCols.ContainsKey(t) ? DbCols[t] : null;

                if (!(existing is null))
                    return existing;

                var dbCol = new DbCollection
                {
                    DatabaseId = options.DatabaseId ?? throw new ArgumentNullException(nameof(options.DatabaseId)),
                    DatabaseThroughput = options.OfferThroughput
                };

                foreach (var p in options.CollectionProperties)
                {
                    if (p.Value.Models is null)
                        throw new ArgumentNullException($"Seems the config is invalid for {p.Key}");

                    if (!p.Value.Models.Any(model => model.EqualsEx(t.Name)))
                        continue;

                    dbCol.ColProps = p.Value;
                    dbCol.CollectionId = p.Key;
                    break;
                }

                if (dbCol.ColProps is null)
                    throw new ArgumentNullException(nameof(IOptions<CosmosDbOptions>),
                        $"CollectionProperty value for '{t.Name}' was not found!"
                    );

                if (dbCol.CollectionId is null)
                    throw new ArgumentNullException(nameof(dbCol.CollectionId));

                dbCol.DocumentCollectionUri = UriFactory.CreateDocumentCollectionUri(dbCol.DatabaseId, dbCol.CollectionId);

                DbCols.Add(t, dbCol);

                return dbCol;
            }
        }

        public static async Task<bool> CreateDbColDatabaseAsync(this IDocumentClient docClient, ILogger logger, DbCollection dbCol,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var exists = await docClient.CreateDatabaseQuery()
                    .Where(d => d.Id == dbCol.DatabaseId)
                    .ToAsyncEnumerable().AnyAsync(cancellationToken);

                if (exists)
                    return false;

                logger.LogWarning(() => $"Creating database {dbCol.DatabaseId} as it does not exist.");

                var options = dbCol.DatabaseThroughput == -1
                    ? null
                    : new RequestOptions
                    {
                        OfferThroughput = dbCol.DatabaseThroughput
                    };

                await docClient.CreateDatabaseAsync(new Database {Id = dbCol.DatabaseId}, options);
            }
            catch (DocumentClientException dce)
            {
                if (dce.StatusCode != HttpStatusCode.Conflict)
                    throw;
            }

            return true;
        }

        public static async Task<bool> CreateDbColCollectionAsync(this IDocumentClient docClient, ILogger logger, DbCollection dbCol,
            CancellationToken cancellationToken = default
        )
        {
            var defaultIndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) {Precision = -1});

            var docCol = new DocumentCollection
            {
                Id = dbCol.CollectionId,
                IndexingPolicy = defaultIndexingPolicy,
                UniqueKeyPolicy = dbCol.ColProps.UniqueKeyPolicy,
                PartitionKey = new PartitionKeyDefinition
                {
                    Paths = new Collection<string> {"/StreamId"}
                }
            };

            var p = new IncludedPath { Path = "/StreamId/?" };
            docCol.IndexingPolicy.IncludedPaths.Add(p);

            p = new IncludedPath { Path = "/DocumentType/?" };
            docCol.IndexingPolicy.IncludedPaths.Add(p);

            var existingCol = await docClient.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(dbCol.DatabaseId))
                .Where(d => d.Id == dbCol.CollectionId)
                .ToAsyncEnumerable()
                .SingleOrDefaultAsync(cancellationToken);

            var options = new RequestOptions
            {
                OfferThroughput = dbCol.ColProps.OfferThroughput > -1
                    ? dbCol.ColProps.OfferThroughput
                    : (int?) null
            };

            if (!(existingCol is null))
                return false;

            logger.LogWarning(() => $"Creating collection {dbCol.CollectionId} as it does not exist.");

            var retryCount = 0;

            while (true)
                try
                {
                    var res = await docClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(dbCol.DatabaseId),
                        docCol,
                        options
                    );

                    Console.WriteLine(res.Resource);

                    break;
                }
                catch (DocumentClientException dce)
                {
                    if (dce.Error.Code == "449")
                    {
                        if (retryCount > 4)
                            throw;

                        await Task.Delay(250, cancellationToken);
                        retryCount++;

                        continue;
                    }

                    if (dce.StatusCode != HttpStatusCode.Conflict)
                        throw;

                    break;
                }

            return true;
        }
    }
}
