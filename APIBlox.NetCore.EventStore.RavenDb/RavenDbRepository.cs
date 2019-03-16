using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using Newtonsoft.Json;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

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

        public async Task<int> AddAsync<TDocument>(TDocument[] documents, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument
        {
            using (var session = _context.Store(_colName).OpenAsyncSession(new SessionOptions { NoCaching = true }))
            {
                foreach (var document in documents)
                    await session.StoreAsync(document, document.Id, cancellationToken);

                await session.SaveChangesAsync(cancellationToken);
            }

            return documents.Length;
        }

        public async Task<IEnumerable<TResultDocument>> GetAsync<TResultDocument>(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
            where TResultDocument : EventStoreDocument
        {
            IEnumerable<TResultDocument> ret;

            using (var session = _context.Store(_colName).OpenAsyncSession(new SessionOptions { NoCaching = true }))
            {
                ret = await session.Query<EventStoreDocument>(null, _colName)
                    .Where(predicate)
                    .OfType<TResultDocument>()
                    .ToListAsync(token: cancellationToken);
            }

            return ret;
        }

        public async Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument
        {
            using (var session = _context.Store(_colName).OpenAsyncSession(new SessionOptions { NoCaching = true }))
            {
                var doc = await session.Query<TDocument>(collectionName: _colName)
                    .SingleAsync(x => x.Id == document.Id, token: cancellationToken);

                doc.Version = document.Version;

                await session.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<int> DeleteAsync(Expression<Func<EventStoreDocument, bool>> predicate, CancellationToken cancellationToken = default)
        {
            int ret;

            using (var session = _context.Store(_colName).OpenAsyncSession(new SessionOptions { NoCaching = true }))
            {
                var ids = await session.Query<EventStoreDocument>(null, _colName)
                    .Where(predicate)
                    .Select(x => x.Id)
                    .ToListAsync(token: cancellationToken);

                ret = ids.Count;

                foreach (var id in ids)
                    session.Delete(id);

                await session.SaveChangesAsync(cancellationToken);
            }

            return ret;
        }
    }
}
