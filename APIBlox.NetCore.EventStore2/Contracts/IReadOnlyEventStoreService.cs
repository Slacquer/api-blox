//#region -    Using Statements    -

//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using APIBlox.NetCore.Models;
//using Newtonsoft.Json;

//#endregion

//namespace APIBlox.NetCore.Contracts
//{
//    /// <summary>
//    ///     Marker interface
//    /// </summary>
//    /// <typeparam name="TAggregate">The type of the t aggregate.</typeparam>
//    /// <seealso cref="IReadOnlyEventStoreService" />
//    public interface IReadOnlyEventStoreService<TAggregate> : IReadOnlyEventStoreService
//        where TAggregate : class
//    {
//    }

//    /// <summary>
//    ///     Interface IReadOnlyEventStoreService
//    /// </summary>
//    /// <seealso cref="IReadOnlyEventStoreService" />
//    public interface IReadOnlyEventStoreService
//    {
//        /// <summary>
//        ///     Gets or sets the json settings.
//        /// </summary>
//        /// <value>The json settings.</value>
//        JsonSerializerSettings JsonSettings { get; set; }

        
//        Task<EventStreamModel> GetEventStreamAsync(string streamId, long? fromVersion = null,
//            Func<object> initializeSnapshotObject = null, CancellationToken cancellationToken = default
//        );
//    }
//}
