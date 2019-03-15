using System;
using System.Linq;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace APIBlox.NetCore.EventStore
{
    internal class CollectionContext
    {
        private readonly IMongoDatabase _database;

        public CollectionContext(string cnnStr, string databaseId)
        {
            var client = new MongoClient(cnnStr);

            _database = client.GetDatabase(databaseId);

            BuildEventStoreDocumentMaps();
        }

        public IMongoCollection<TDocument> Collection<TDocument>(string colName)
        {
            var ret =  _database.GetCollection<TDocument>(colName);
            
            return ret;
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
