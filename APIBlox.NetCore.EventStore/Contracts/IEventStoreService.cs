﻿using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Models;

namespace APIBlox.NetCore.Contracts
{
    /// <inheritdoc />
    /// <summary>
    ///     Interface IEventStoreService
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.  This is a marker, to allow multiple instances.</typeparam>
    public interface IEventStoreService<TModel> : IReadOnlyEventStoreService<TModel>
        where TModel : class
    {
        /// <summary>
        ///     Writes to event stream asynchronous.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="events">The events.</param>
        /// <param name="metaData">The meta data.</param>
        /// <param name="expectedVersion">The expected version.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;EventStreamModel&gt;.</returns>
        Task<EventStreamModel> WriteToEventStreamAsync(string streamId, EventModel[] events,
            object metaData = null,
            long? expectedVersion = null, CancellationToken cancellationToken = default
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
