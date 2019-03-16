using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raven.Client.Documents.Linq;

namespace APIBlox.NetCore
{
    internal class RavenDbRepository<TModel> : IEventStoreRepository<TModel>
        where TModel : class
    {
        private readonly string _colName;
        private readonly StoreContext _context;

        public RavenDbRepository(StoreContext context, JsonSerializerSettings serializerSettings)
        {
            _context = context;
            JsonSettings = serializerSettings ?? throw new ArgumentNullException(nameof(serializerSettings));

            _colName = typeof(TModel).Name;
        }

        public JsonSerializerSettings JsonSettings { get; }

        public Task<int> AddAsync<TDocument>(TDocument[] documents, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument
        {
            using (var session = _context.Store(_colName).OpenSession())
            {
                foreach (var document in documents)
                    session.Store(document, document.Id);

                session.SaveChanges();
            }

            return Task.FromResult(documents.Length);
        }

        public Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
            where TResult : EventStoreDocument
        {
            using (var session = _context.Store(_colName).OpenSession())
            {
                var ret = session.Query<EventStoreDocument>(null, _colName)
                    .Where(predicate)
                    .OfType<TResult>().ToList();

                return Task.FromResult<IEnumerable<TResult>>(ret);
            }
        }

        public Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument
        {
            using (var session = _context.Store(_colName).OpenSession())
            {
                var doc = session.Query<TDocument>(collectionName: _colName)
                    .Single(x => x.Id == document.Id
                    );
                doc.Version = document.Version;

                session.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task<int> DeleteAsync(Expression<Func<EventStoreDocument, bool>> predicate, CancellationToken cancellationToken = default)
        {
            int ret;

            using (var session = _context.Store(_colName).OpenSession())
            {
                var ids = session.Query<EventStoreDocument>(null, _colName)
                    .Where(predicate)
                    .Select(x => x.Id)
                    .ToList();

                ret = ids.Count;

                foreach (var id in ids)
                    session.Delete(id);

                session.SaveChanges();
            }

            return Task.FromResult(ret);
        }
    }
}
