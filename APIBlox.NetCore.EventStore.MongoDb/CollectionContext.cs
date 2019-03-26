using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace APIBlox.NetCore
{
    internal class CollectionContext
    {
        private readonly ConcurrentBag<string> _createdIndexes = new ConcurrentBag<string>();
        private readonly IMongoDatabase _database;
        private readonly MongoDbOptions _options;

        public CollectionContext(MongoDbOptions options)
        {
            _options = ValidateOptions(options);

            var client = new MongoClient(_options.CnnString);

            _database = client.GetDatabase(_options.DatabaseId);

            BuildEventStoreDocumentMaps();
        }

        public IMongoCollection<TDocument> Collection<TDocument>(string colName)
            where TDocument : IEventStoreDocument
        {
            var ret = _database.GetCollection<TDocument>(colName);

            IndexCheck(ret);

            return ret;
        }

        private void IndexCheck<TDocument>(IMongoCollection<TDocument> collection)
            where TDocument : IEventStoreDocument
        {
            if(_options.CollectionProperties is null || !_options.CollectionProperties.Any())
                return;

            var cn = collection.CollectionNamespace.CollectionName;

            if (_createdIndexes.Contains(cn))
                return;

            Task.Run(async () =>
                {
                    var props = _options.CollectionProperties.FirstOrDefault(k => k.Key == cn).Value;

                    if (props.Indexes is null || !props.Indexes.Any())
                        return;

                    var lst = _options.CollectionProperties.FirstOrDefault(k => k.Key == cn).Value.Indexes
                        .Select(index => new CreateIndexModel<TDocument>(index))
                        .ToList();

                    await collection.Indexes.CreateManyAsync(lst);

                    _createdIndexes.Add(cn);
                }
            ).Wait();
        }

        private MongoDbOptions ValidateOptions(MongoDbOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (options.CnnString.IsEmptyNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(options.CnnString));

            if (options.DatabaseId.IsEmptyNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(options.DatabaseId));

            return options;
        }

        private static void BuildEventStoreDocumentMaps()
        {
            var props = typeof(EventStoreDocument).JsonPropertyNames();

            BsonClassMap.RegisterClassMap<EventStoreDocument>(c =>
                {
                    c.AutoMap();

                    c.MapIdMember(p => p.Id);

                    c.SetIsRootClass(true);

                    c.MapMember(m => m.DocumentType).SetSerializer(new EnumSerializer<DocumentType>(BsonType.String));

                    foreach (var (key, value) in props.Where(p => p.Value != "id"))
                        c.MapProperty(key.Name).SetElementName(value);
                }
            );

            // Other documents are internal, so we will add them manually.
            var mapAble = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(EventStoreDocument).IsAssignableFrom(x)
                            && !x.IsInterface
                            && !x.IsAbstract
                            && x.Name != nameof(EventStoreDocument)
                );

            foreach (var type in mapAble)
                BsonClassMap.RegisterClassMap(new BsonClassMap(type));
        }
    }
}
