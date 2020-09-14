using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Documents;
using Newtonsoft.Json;

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    /// Interface IEventStoreRepository
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.  This is a marker, to allow multiple instances.</typeparam>
    public interface IEventStoreRepository<TModel>
        where TModel : class
    {
        /// <summary>
        ///     Adds the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="documents">The documents.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> AddAsync<TDocument>(TDocument[] documents, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument;

        /// <summary>
        ///     Gets the stored document asynchronously.
        /// </summary>
        /// <typeparam name="TResultDocument">The type of the t result.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResultDocument&gt;&gt;.</returns>
        Task<IEnumerable<TResultDocument>> GetAsync<TResultDocument>(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )
            where TResultDocument : EventStoreDocument;

        /// <summary>
        ///     Updates the asynchronous.
        /// </summary>
        /// <param name="document">The event object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument;

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> DeleteAsync(Expression<Func<EventStoreDocument, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
