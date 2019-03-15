using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace APIBlox.NetCore
{
    internal class MongoDbRepository<TModel> : IEventStoreRepository<TModel>
        where TModel : class
    {
        private readonly string _colName;
        private readonly CollectionContext _context;

        public MongoDbRepository(CollectionContext context, JsonSerializerSettings serializerSettings)
        {
            _context = context;

            _colName = typeof(TModel).Name;
            
            JsonSettings = serializerSettings ?? throw new ArgumentNullException(nameof(serializerSettings));
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
    }
}
