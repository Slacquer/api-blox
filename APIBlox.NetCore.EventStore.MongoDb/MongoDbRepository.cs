using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.EventStore.MongoDb.Options;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIBlox.NetCore.EventStore.MongoDb
{
    internal class MongoDbRepository<TModel> : IEventStoreRepository
    {
        private readonly CollectionContext _context;
        public JsonSerializerSettings JsonSettings { get; set; }

        private readonly string _colName;

        public MongoDbRepository(CollectionContext context)
        {
            _context = context;

            _colName = typeof(TModel).Name;

            BsonClassMap.RegisterClassMap<EventStoreDocument>();
            //c =>
            //{
            //    c.AutoMap();
            //    c.MapProperty(p => p.Id).SetElementName("_id");
            //});
            BsonClassMap.RegisterClassMap<RootDocument>();
            BsonClassMap.RegisterClassMap<SnapshotDocument>();
            BsonClassMap.RegisterClassMap<EventDocument>();



            SetJsonSettings();
        }

        private void SetJsonSettings()
        {
            if (!(JsonSettings is null))
                return;

            var tmp = new CamelCaseSettings();
            tmp.Converters.Add(new StringEnumConverter());

            JsonSettings = tmp;
        }

        public async Task<int> AddAsync<TDocument>(TDocument[] documents,
            CancellationToken cancellationToken = default)
            where TDocument : IEventStoreDocument
        {
            await _context.Collection<TDocument>(_colName).InsertManyAsync(documents, null, cancellationToken);

            return documents.Length;
        }

        public Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<IEventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default)
            where TResult : class
        {
            var ret = _context.Collection<IEventStoreDocument>(_colName)
                .AsQueryable().Where(predicate.Compile());

            var isString = typeof(TResult) == typeof(string);

            var lst = ret.Select(document => isString
                ? JsonConvert.SerializeObject(document, JsonSettings) as TResult
                : JsonConvert.DeserializeObject<TResult>(JsonConvert.SerializeObject(document, JsonSettings), JsonSettings)
            ).ToList();

            //http://blog.i3arnon.com/2015/12/16/async-linq-to-objects-over-mongodb/
            return Task.FromResult(lst.AsEnumerable());
        }

        public async Task UpdateAsync<TDocument>(TDocument document,
            CancellationToken cancellationToken = default)
            where TDocument : IEventStoreDocument
        {
            await _context.Collection<EventStoreDocument>(_colName)
                .ReplaceOneAsync(i => i.Id == document.Id, document as EventStoreDocument, null, cancellationToken);
        }

        public async Task<int> DeleteAsync(Expression<Func<IEventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            
            var ret = await _context.Collection<EventStoreDocument>(_colName)
                .DeleteManyAsync(predicate, null, cancellationToken);

            return (int)ret.DeletedCount;
        }
    }
}
