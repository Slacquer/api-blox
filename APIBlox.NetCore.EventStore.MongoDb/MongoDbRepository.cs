using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Types.JsonBits;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIBlox.NetCore.EventStore.MongoDb
{
    internal class MongoDbRepository<TModel> : IEventStoreRepository
    {
        private readonly string _colName;
        private readonly CollectionContext _context;

        public MongoDbRepository(CollectionContext context)
        {
            _context = context;

            _colName = typeof(TModel).Name;

            BsonClassMap.RegisterClassMap<EventStoreDocument>(c =>
                {
                    c.AutoMap();

                    c.MapProperty(p => p.Id).SetElementName("_id");
                    c.MapProperty(p => p.SortOrder);
                }
            );

            SetJsonSettings();
        }

        public JsonSerializerSettings JsonSettings { get; set; }

        public async Task<int> AddAsync<TDocument>(TDocument[] documents,
            CancellationToken cancellationToken = default
        )
            where TDocument : EventStoreDocument
        {
            await _context.Collection<TDocument>(_colName).InsertManyAsync(
                documents,
                new InsertManyOptions {IsOrdered = true},
                cancellationToken
            );

            return documents.Length;
        }

        public async Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
            where TResult : class
        {
            var ret = await _context.Collection<EventStoreDocument>(_colName).Find(predicate).ToListAsync(cancellationToken);

            if (typeof(TResult) == typeof(EventStoreDocument))
                return (IEnumerable<TResult>) ret;

            var isString = typeof(TResult) == typeof(string);

            var lst = ret.Select(document => isString
                ? JsonConvert.SerializeObject(document, JsonSettings) as TResult
                : JsonConvert.DeserializeObject<TResult>(JsonConvert.SerializeObject(document, JsonSettings), JsonSettings)
            ).ToList();

            return lst;
        }

        public async Task UpdateAsync<TDocument>(TDocument document,
            CancellationToken cancellationToken = default
        )
            where TDocument : EventStoreDocument
        {
            await _context.Collection<EventStoreDocument>(_colName).ReplaceOneAsync(
                i => i.Id == document.Id,
                document,
                new UpdateOptions {IsUpsert = false},
                cancellationToken
            );
        }

        public async Task<int> DeleteAsync(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
        {
            var ret = await _context.Collection<EventStoreDocument>(_colName)
                .DeleteManyAsync(predicate, null, cancellationToken);

            return (int) ret.DeletedCount;
        }

        private void SetJsonSettings()
        {
            if (!(JsonSettings is null))
                return;

            var tmp = new CamelCaseSettings();
            tmp.Converters.Add(new StringEnumConverter());

            JsonSettings = tmp;
        }
    }
}
