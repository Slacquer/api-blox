using System;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace APIBlox.NetCore
{
    internal class StoreContext : IDisposable
    {
        private readonly IDocumentStore _store;
        private string _collection;

        public StoreContext(RavenDbOptions options)
        {
            var opt = options ?? throw new ArgumentNullException(nameof(options));

            var database = opt.DatabaseId.IsEmptyNullOrWhiteSpace() ? throw new ArgumentNullException(nameof(opt.DatabaseId)) : opt.DatabaseId;

            _store = new DocumentStore
            {
                Urls = opt.Urls ?? throw new ArgumentNullException(nameof(opt.Urls)),
                Database = database
            };

            _store.Conventions.FindCollectionName = EventStoreCollectionName;

            _store.Conventions.FindIdentityProperty = mi => mi.Name.EqualsEx("id");

            _store.Initialize();

            EnsureDatabaseExists(_store, database);
        }

        public IDocumentStore Store(string collection)
        {
            _collection = collection;

            return _store;
        }

        public void Dispose()
        {
            _store?.Dispose();
        }

        private string EventStoreCollectionName(Type arg)
        {
            return _collection;
        }

        private static void EnsureDatabaseExists(IDocumentStore store, string database = null, bool createDatabaseIfNotExists = true)
        {
            database ??= store.Database;

            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(database));

            try
            {
                store.Maintenance.ForDatabase(database).Send(new GetStatisticsOperation());
            }
            catch (DatabaseDoesNotExistException)
            {
                if (createDatabaseIfNotExists == false)
                    throw;

                try
                {
                    store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(database)));
                }
                catch (ConcurrencyException)
                {
                    // The database was already created before calling CreateDatabaseOperation
                }
            }
        }
    }
}
