using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIBlox.NetCore
{
    internal class EfCoreSqlRepository<TModel> : IEventStoreRepository<TModel>
        where TModel : class
    {
        private readonly EventStoreDbContext _context;
        private readonly JsonSerializerSettings _jsonSettings;

        public EfCoreSqlRepository(EventStoreDbContext context, IEventStoreJsonSerializerSettings jsonSerializerSettings)
        {
            _context = context;
            _jsonSettings = jsonSerializerSettings.Settings;
        }

        public async Task<int> AddAsync<TDocument>(TDocument[] documents, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument
        {
            foreach (var document in documents)
            {
                await _context.Documents.AddAsync(new DocEx
                {
                    Data = JsonConvert.SerializeObject(document.Data, _jsonSettings),
                    DataType = document.DataType,
                    DocumentType = document.DocumentType,
                    Id = document.Id,
                    StreamId = document.StreamId,
                    TimeStamp = document.TimeStamp,
                    Version = document.Version
                },
                    cancellationToken
                );
            }

            var ret = await _context.SaveChangesAsync(cancellationToken);

            return ret;
        }

        public async Task<IEnumerable<TResultDocument>> GetAsync<TResultDocument>(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
            where TResultDocument : EventStoreDocument
        {
            var ret = await _context.Documents.Where(predicate).ToListAsync(cancellationToken);

            var lst = ret.Cast<DocEx>()
                .Select(ex => new EventStoreDocument
                {
                    Data = ex.DataType is null ? null : JObject.Parse(ex.Data).ToObject(Type.GetType(ex.DataType)!),
                    DataType = ex.DataType,
                    DocumentType = ex.DocumentType,
                    Id = ex.Id,
                    StreamId = ex.StreamId,
                    TimeStamp = ex.TimeStamp,
                    Version = ex.Version
                }
                ).ToList();

            return (IEnumerable<TResultDocument>)lst;
        }

        public async Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument
        {
            var doc = _context.Documents.Single(d => d.Id == document.Id);

            doc.Version = document.Version;
            doc.TimeStamp = document.TimeStamp;

            _context.Update(doc);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> DeleteAsync(Expression<Func<EventStoreDocument, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var docs = await _context.Documents.Where(predicate).ToListAsync(cancellationToken);

            foreach (var doc in docs)
                _context.Remove(doc);

            var ret = await _context.SaveChangesAsync(cancellationToken);

            return ret;
        }
    }
}
