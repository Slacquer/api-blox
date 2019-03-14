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
    ///     Interface IEventStoreRepository
    /// </summary>
    public interface IEventStoreRepository
    {
        /// <summary>
        ///     Gets or sets the json settings.
        /// </summary>
        /// <value>The json settings.</value>
        JsonSerializerSettings JsonSettings { get; set; }

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
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<EventStoreDocument, bool>> predicate,
            CancellationToken cancellationToken = default
        )where TResult : class;

        /// <summary>
        ///     Updates the asynchronous.
        /// </summary>
        /// <param name="document">The event object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
            where TDocument : EventStoreDocument;

        /// <summary>
        ///     Deletes the asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> DeleteAsync(Expression<Func<EventStoreDocument, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
