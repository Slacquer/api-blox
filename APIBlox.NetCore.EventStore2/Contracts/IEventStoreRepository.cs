using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace APIBlox.NetCore.Contracts
{
    public interface IEventStoreRepository<TDocument>
        where TDocument : IEventStoreDocument
    {
        Task<int> AddAsync(params TDocument[] eventObject);

        Task<TDocument> GetByIdAsync(string streamId);

        Task<IEnumerable<TDocument>> GetAsync(Func<TDocument, bool> predicate);

        Task UpdateAsync(TDocument eventObject);

        Task<bool> DeleteAsync(Func<TDocument, bool> predicate);
    }
}
