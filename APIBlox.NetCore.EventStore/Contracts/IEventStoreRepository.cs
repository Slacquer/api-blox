using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Interface IEventStoreRepository
    /// </summary>
    public interface IEventStoreRepository

    {
        /// <summary>
        ///     Adds the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="documents">The documents.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> AddAsync<TDocument>(TDocument[] documents, CancellationToken cancellationToken = default)
            where TDocument : IEventStoreDocument;

        ///// <summary>
        /////     Gets the by identifier asynchronous.
        ///// </summary>
        ///// <param name="id">The identifier.</param>
        ///// <param name="cancellationToken">The cancellation token.</param>
        ///// <returns>Task&lt;TDocument&gt;.</returns>
        //Task<TDocument> GetByIdAsync<TDocument>(string id, CancellationToken cancellationToken = default)
        //    where TDocument : IEventStoreDocument;

        ///// <summary>
        /////     Gets the by stream identifier asynchronous.
        ///// </summary>
        ///// <param name="streamId">The identifier.</param>
        ///// <param name="cancellationToken">The cancellation token.</param>
        ///// <returns>Task&lt;TDocument&gt;.</returns>
        //Task<TDocument> GetByStreamIdAsync<TDocument>(string streamId, CancellationToken cancellationToken = default)
        //    where TDocument : IEventStoreDocument;

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="TDocument">The type of the t document.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;Document&gt;&gt;.</returns>
        Task<IEnumerable<Document>> GetAsync<TDocument>(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default)
            where TDocument : IEventStoreDocument;

        /// <summary>
        ///     Updates the asynchronous.
        /// </summary>
        /// <param name="document">The event object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
            where TDocument : IEventStoreDocument;


        ///// <summary>
        ///// Deletes the by identifier asynchronous.
        ///// </summary>
        ///// <typeparam name="TDocument">The type of the t document.</typeparam>
        ///// <param name="id">The identifier.</param>
        ///// <param name="cancellationToken">The cancellation token.</param>
        ///// <returns>Task&lt;TDocument&gt;.</returns>
        //Task<TDocument> DeleteByIdAsync<TDocument>(string id, CancellationToken cancellationToken = default)
        //    where TDocument : IEventStoreDocument;

        ///// <summary>
        ///// Deletes the by stream identifier asynchronous.
        ///// </summary>
        ///// <typeparam name="TDocument">The type of the t document.</typeparam>
        ///// <param name="streamId">The stream identifier.</param>
        ///// <param name="cancellationToken">The cancellation token.</param>
        ///// <returns>Task&lt;TDocument&gt;.</returns>
        //Task<TDocument> DeleteByStreamIdAsync<TDocument>(string streamId, CancellationToken cancellationToken = default)
        //    where TDocument : IEventStoreDocument;

        /// <summary>
        ///     Deletes the asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> DeleteAsync<TDocument>(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default)
            where TDocument : IEventStoreDocument;

        JsonSerializerSettings JsonSettings { get; set; }
    }
}
