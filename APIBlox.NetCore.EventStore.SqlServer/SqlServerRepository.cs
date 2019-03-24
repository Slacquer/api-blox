using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using Newtonsoft.Json;

namespace APIBlox.NetCore
{
    internal class SqlServerRepository<TModel>:IEventStoreRepository<TModel>
        where TModel : class
    {
        private readonly SqlDbContext _context;

        public SqlServerRepository(SqlDbContext context)
        {
            _context = context;
        }
        public JsonSerializerSettings JsonSettings { get; }

        public Task<int> AddAsync<TDocument>(TDocument[] documents, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TResultDocument>> GetAsync<TResultDocument>(Expression<Func<EventStoreDocument, bool>> predicate, CancellationToken cancellationToken = default)
            where TResultDocument : EventStoreDocument
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Expression<Func<EventStoreDocument, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
