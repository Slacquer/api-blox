using System;
using System.Collections.Generic;
using System.Threading;
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="documents">The documents.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> AddAsync(CancellationToken cancellationToken = default, params TDocument[] documents);

        /// <summary>
        ///     Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;TDocument&gt;.</returns>
        Task<TDocument> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Gets the by stream identifier asynchronous.
        /// </summary>
        /// <param name="streamId">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;TDocument&gt;.</returns>
        Task<TDocument> GetByStreamIdAsync(string streamId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Gets the asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;TDocument&gt;&gt;.</returns>
        Task<IEnumerable<TDocument>> GetAsync(Func<TDocument, bool> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Updates the asynchronous.
        /// </summary>
        /// <param name="document">The event object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task UpdateAsync(TDocument document, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Deletes the asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> DeleteAsync(Func<TDocument, bool> predicate, CancellationToken cancellationToken = default);
    }
}
