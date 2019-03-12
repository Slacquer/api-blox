using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Interface IEventStoreRepository
    /// </summary>
    /// <typeparam name="TDocument">The type of the t document.</typeparam>
    public interface IEventStoreRepository<TDocument>
        where TDocument : IEventStoreDocument
    {
        /// <summary>
        ///     Adds the asynchronous.
        /// </summary>
        /// <param name="eventObject">The event object.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> AddAsync(params TDocument[] eventObject);

        /// <summary>
        ///     Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;TDocument&gt;.</returns>
        Task<TDocument> GetByIdAsync(string id);

        /// <summary>
        ///     Gets the asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Task&lt;IEnumerable&lt;TDocument&gt;&gt;.</returns>
        Task<IEnumerable<TDocument>> GetAsync(Func<TDocument, bool> predicate);

        /// <summary>
        ///     Updates the asynchronous.
        /// </summary>
        /// <param name="eventObject">The event object.</param>
        /// <returns>Task.</returns>
        Task UpdateAsync(TDocument eventObject);

        /// <summary>
        ///     Deletes the asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> DeleteAsync(Func<TDocument, bool> predicate);
    }
}
