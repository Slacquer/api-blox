using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Models;

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Interface IReadOnlyEventStoreService
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.  This is a marker, to allow multiple instances.</typeparam>
    public interface IReadOnlyEventStoreService<TModel>
        where TModel : class
    {
        /// <summary>
        ///     Reads the stream asynchronous.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Version and Timestamp, when empty will return null for both.</returns>
        Task<(long?, DateTimeOffset?)> ReadEventStreamVersionAsync(string streamId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        ///     Reads the stream asynchronously.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;EventStreamModel&gt;.</returns>
        Task<EventStreamModel> ReadEventStreamAsync(string streamId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        ///     Reads the stream asynchronously optionally from a specific version.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="fromVersion">From version.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;EventStreamModel&gt;.</returns>
        Task<EventStreamModel> ReadEventStreamAsync(string streamId, long fromVersion,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        ///     Reads the event stream asynchronously from a specific date optionally to a specific date.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;EventStreamModel&gt;.</returns>
        Task<EventStreamModel> ReadEventStreamAsync(string streamId, DateTimeOffset fromDate,
            DateTimeOffset? toDate = null,
            CancellationToken cancellationToken = default
        );


        /// <summary>
        ///     Reads the event stream asynchronously optionally using an express.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="predicate">The predicate.  Will always contain streamId == streamId</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;EventStreamModel&gt;.</returns>
        Task<EventStreamModel> ReadEventStreamAsync(string streamId,
            Expression<Func<EventStoreDocument, bool>> predicate = null,
            CancellationToken cancellationToken = default
        );
    }
}
