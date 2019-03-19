using System;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Models;

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Interface IReadOnlyEventStoreService
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    public interface IReadOnlyEventStoreService<TModel>
        where TModel : class
    {
        /// <summary>
        ///     Reads the stream asynchronous.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;EventStreamModel&gt;.</returns>
        Task<(long, string)> ReadEventStreamVersionAsync(string streamId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        ///     Reads the stream asynchronous.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="fromVersion">From version.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;EventStreamModel&gt;.</returns>
        Task<EventStreamModel> ReadEventStreamAsync(string streamId, long? fromVersion = null,
            CancellationToken cancellationToken = default
        );
    }
}
