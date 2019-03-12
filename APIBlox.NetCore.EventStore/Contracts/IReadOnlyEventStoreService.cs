using System;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Models;
using Newtonsoft.Json;

namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Marker interface
    /// </summary>
    /// <typeparam name="TModel">The type of the t aggregate.</typeparam>
    /// <seealso cref="IReadOnlyEventStoreService" />
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
        ///     Gets or sets the json settings.
        /// </summary>
        /// <value>The json settings.</value>
        JsonSerializerSettings JsonSettings { get; set; }

        /// <summary>
        ///     Reads the stream asynchronous.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="fromVersion">From version.</param>
        /// <param name="includeEvents">if set to <c>true</c> [include events].</param>
        /// <param name="initializeSnapshotObject">The initialize snapshot object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;EventStreamModel&gt;.</returns>
        Task<EventStreamModel> ReadEventStreamAsync(string streamId, long? fromVersion = null,
            bool includeEvents = false, Func<object> initializeSnapshotObject = null, CancellationToken cancellationToken = default
        );

        ///// <summary>
        /////     Gets the stream root asynchronous.
        ///// </summary>
        ///// <param name="streamId">The stream identifier.</param>
        ///// <param name="partitionedByValue">The partitioned by value.</param>
        ///// <param name="cancellation">The cancellation.</param>
        ///// <returns>Task&lt;EventStreamModel&gt;.</returns>
        //Task<EventStreamModel> GetEventStreamRootAsync(string streamId, string partitionedByValue, CancellationToken cancellation = default);

        ///// <summary>
        /////     Gets all partition key values asynchronous.
        ///// </summary>
        ///// <param name="cancellationToken">The cancellation token.</param>
        ///// <returns>Task&lt;IEnumerable&lt;System.String&gt;&gt;.</returns>
        //Task<IEnumerable<string>> GetAllPartitionKeyValuesInCollectionAsync(CancellationToken cancellationToken = default);
    }
}
