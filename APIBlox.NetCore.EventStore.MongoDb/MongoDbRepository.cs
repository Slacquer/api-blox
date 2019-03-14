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

            JsonSettings =  tmp;
        }

        public async Task<int> AddAsync<TDocument>(TDocument[] documents,
            CancellationToken cancellationToken = default)
            where TDocument : IEventStoreDocument
        {
           await _context.Collection<TDocument>(_colName).InsertManyAsync(documents,null, cancellationToken);

           return documents.Length;
        }

        public  Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<IEventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default)
            where TResult : class
        {
            var qry = _context.Collection<IEventStoreDocument>(_colName).AsQueryable();

            var ret = qry.Where(predicate.Compile());

            //var ret = await _context.Collection<IEventStoreDocument>(_colName).Find(predicate).ToListAsync(cancellationToken);

            //var ret = await _context.Collection<IEventStoreDocument>(_colName).Find(predicate).ToListAsync(cancellationToken);
            //var ret = await _context.Collection<IEventStoreDocument>(_colName).FindAsync(predicate, null, cancellationToken);

            var isString = typeof(TResult) == typeof(string);

            var lst = new List<TResult>();

            foreach (var document in ret) //await ret.ToListAsync(cancellationToken))
            {
                var result = isString
                    ? JsonConvert.SerializeObject(document, JsonSettings) as TResult
                    : JsonConvert.DeserializeObject<TResult>(JsonConvert.SerializeObject(document, JsonSettings), JsonSettings);

                lst.Add(result);
            }
//http://blog.i3arnon.com/2015/12/16/async-linq-to-objects-over-mongodb/
            return Task.FromResult( lst.AsEnumerable());
        }

        public Task UpdateAsync<TDocument>(TDocument document,
            CancellationToken cancellationToken = default)
            where TDocument : IEventStoreDocument
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Expression<Func<IEventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
