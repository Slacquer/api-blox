//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace AEU.Persistance.Contracts
//{
//    public interface ICosmosDbRepository<T> 
//        where T : class
//    {
//        Task<int> AddAsync(params T[] entity);

//        Task<IEnumerable<T>> GetAsync(Expression<Func<T,bool>> predicate);

//        Task<T> UpdateAsync(T entity);

//        Task<bool> DeleteAsync(Expression<Func<T,bool>> predicate);
//    }
//}
