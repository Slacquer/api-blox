using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Models;

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Marker Interface
    /// </summary>
    /// <typeparam name="TModel">The type of the t aggregate.</typeparam>
    /// <seealso cref="IReadOnlyEventStoreService{TModel}" />
    /// <seealso cref="IEventStoreService" />
    public interface IEventStoreService<TModel> : IReadOnlyEventStoreService<TModel>, IEventStoreService
        where TModel : class
    {
    }

    /// <summary>
    ///     Interface IEventStoreService
    /// </summary>
    /// <seealso cref="IReadOnlyEventStoreService{TModel}" />
    /// <seealso cref="IEventStoreService" />
    public interface IEventStoreService : IReadOnlyEventStoreService
    {
        /// <summary>
        ///     Writes to stream asynchronously.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="events">The events.</param>
        /// <param name="expectedVersion">The expected version.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.UInt64&gt;.</returns>
        Task<long> WriteToEventStreamAsync(string streamId, EventModel[] events,
            long? expectedVersion = null, object metadata = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        ///     Deletes the stream asynchronous.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task DeleteEventStreamAsync(string streamId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Creates the snapshot asynchronous.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="expectedVersion">The expected version.</param>
        /// <param name="snapshot">The snapshot.</param>
        /// <param name="deleteOlderSnapshots">if set to <c>true</c> [delete older snapshots].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task CreateSnapshotAsync(string streamId, long expectedVersion,
            SnapshotModel snapshot, bool deleteOlderSnapshots = false,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        ///     Deletes the snapshots asynchronous.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="olderThanVersion">The older than version.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task DeleteSnapshotsAsync(string streamId, long olderThanVersion,
            CancellationToken cancellationToken = default
        );
    }
}
