using System;
using System.Collections.Generic;
using System.Text;
using APIBlox.NetCore.EventStore.MongoDb.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace APIBlox.NetCore.EventStore.MongoDb
{
    internal class CollectionContext
    {
        private readonly IMongoDatabase _database;

        public CollectionContext(IOptions<MongoDbOptions> options)
        {
            var opts = options.Value;
            
            var client = new MongoClient(opts.ConnectionString);

            _database = client.GetDatabase(opts.DatabaseId);
        }

        public IMongoCollection<TDocument> Collection<TDocument>(string colName)
        {
            return  _database.GetCollection<TDocument>(colName);
        }
        
    }
}
