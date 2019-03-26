using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Types;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APIBlox.NetCore
{
    internal class EfCoreSqlRepository<TModel> : IEventStoreRepository<TModel>
        where TModel : class
    {
        private readonly EventStoreDbContext _context;

        public EfCoreSqlRepository(EventStoreDbContext context, JsonSerializerSettings jsonSerializerSettings)
        {
            _context = context;
        }
        public JsonSerializerSettings JsonSettings { get; }

        public async Task<int> AddAsync<TDocument>(TDocument[] documents, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument
        {
            foreach (var document in documents)
            {
                var doc = JsonConvert.DeserializeObject<EventStoreDocumentEx>(JsonConvert.SerializeObject(document));

                if (!(document.Data is null))
                    doc.Data = JsonConvert.SerializeObject(document.Data);

                _context.Documents.Add(doc);
            }

            var ret = await _context.SaveChangesAsync(cancellationToken);

            return ret;
        }

        public async Task<IEnumerable<TResultDocument>> GetAsync<TResultDocument>(Expression<Func<IEventStoreDocument, bool>> predicate, CancellationToken cancellationToken = default)
            where TResultDocument : IEventStoreDocument
        {
            var ret = await _context.Documents.Where(predicate).ToListAsync(cancellationToken);

            return (IEnumerable<TResultDocument>)ret;
        }


        public async Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument
        {
            var doc = await _context.Documents.SingleAsync(d => d.Id == document.Id, cancellationToken);

            doc.Version = document.Version;
            doc.TimeStamp = document.TimeStamp;

            _context.Update(doc);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> DeleteAsync(Expression<Func<IEventStoreDocument, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var docs = await _context.Documents.Where(predicate).ToListAsync(cancellationToken);

            foreach (var doc in docs)
            {
                _context.Remove(doc);
            }

            var ret = await _context.SaveChangesAsync(cancellationToken);

            return ret;
        }
    }

}
