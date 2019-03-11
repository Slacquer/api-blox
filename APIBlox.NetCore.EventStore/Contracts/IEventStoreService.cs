#region -    Using Statements    -

using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Models;

#endregion

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Marker Interface
    /// </summary>
    /// <typeparam name="TAggregate">The type of the t aggregate.</typeparam>
    public interface IEventStoreService<TAggregate> : IReadOnlyEventStoreService<TAggregate>, IEventStoreService
        where TAggregate : class
    {
    }

    /// <summary>
    ///     Interface IEventStoreService
    /// </summary>
    public interface IEventStoreService : IReadOnlyEventStoreService
    {
        Task<EventStreamModel> AddStreamAsync(string streamId, object metadata = null);

        Task<int> AddEventsToStreamAsync(string streamId, long expectedVersion, EventModel[] events, CancellationToken cancellationToken = default
        );

        Task DeleteStreamByIdAsync(string streamId, CancellationToken cancellationToken = default);

        Task CreateSnapshotOfStreamAsync(string streamId,long expectedVersion, SnapshotModel snapshot, bool deleteOlderSnapshots = false, CancellationToken cancellationToken = default
        );

        Task DeleteSnapshotsForStreamByIdAsync(string streamId, ulong olderThanVersion,
            CancellationToken cancellationToken = default
        );
    }
}
