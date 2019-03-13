using System;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Models;

namespace APIBlox.NetCore.Contracts
{
    /// <inheritdoc />
    /// <summary>
    ///     Marker interface
    /// </summary>
    /// <typeparam name="TModel">The type of the t aggregate.</typeparam>
    /// <seealso cref="T:APIBlox.NetCore.Contracts.IReadOnlyEventStoreService" />
    public interface IReadOnlyEventStoreService<TModel> : IReadOnlyEventStoreService
        where TModel : class
    {
    }

    /// <summary>
    ///     Interface IReadOnlyEventStoreService
    /// </summary>
    /// <seealso cref="IReadOnlyEventStoreService" />
    public interface IReadOnlyEventStoreService
    {
        /// <summary>
        ///     Reads the stream asynchronous.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="fromVersion">From version.</param>
        /// <param name="includeEvents">if set to <c>true</c> [include events].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;EventStreamModel&gt;.</returns>
        Task<EventStreamModel> ReadEventStreamAsync(string streamId, long? fromVersion = null,
            bool includeEvents = false, CancellationToken cancellationToken = default
        );
    }
}
