//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using APIBlox.NetCore.Contracts;
//using APIBlox.NetCore.Exceptions;
//using APIBlox.NetCore.Models;
//using APIBlox.NetCore.Types.JsonBits;
//using Newtonsoft.Json;

//namespace APIBlox.NetCore
//{
//    public class EventStoreService<TStream> : IEventStoreService<TStream>
//        where TStream : class
//    {
//        private readonly IEventStoreRepository<IEventStoreDocument> _es;
//        public JsonSerializerSettings JsonSettings { get; set; }

//        public EventStoreService(IEventStoreRepository<IEventStoreDocument> eventStoreRepository)
//        {
//            _es = eventStoreRepository;

//            JsonSettings = new JsonSerializerSettings { ContractResolver = new PopulateNonPublicSettersContractResolver() };
//        }

//        public async Task<EventStreamModel> GetEventStreamAsync(string streamId, long? fromVersion = null, Func<object> initializeSnapshotObject = null,
//            CancellationToken cancellationToken = default
//        )
//        {
//            var lst = (await _es.GetAsync(d => d.StreamId == streamId))
//                .OrderByDescending(d => d is EventStoreStreamRootDocument)
//                .ThenBy(d => d.TimeStamp)
//                .ThenBy(d => d is EventStoreSnapshotDocument)
//                .ThenBy(d => d is EventStoreEventDocument).ToList();

//            var root = (EventStoreStreamRootDocument)lst.First();
//            var sn = lst.Where(d => d is EventStoreSnapshotDocument)
//                .Cast<EventStoreSnapshotDocument>()
//                .Select(d => new SnapshotModel(d.SnapshotData))
//                .FirstOrDefault();
//            var events = lst.Where(d => d is EventStoreEventDocument)
//                .Cast<EventStoreEventDocument>()
//                .Select(e => new EventModel(e.EventData));

//            var ret = JsonConvert.DeserializeObject<EventStreamModel>(JsonConvert.SerializeObject(root), JsonSettings);

//            ret.Events = events.ToArray();
//            ret.Snapshot = sn;

//            return ret;
//        }

//        public async Task<EventStreamModel> AddStreamAsync(string streamId, object metadata = null)
//        {
//            var root = new EventStoreStreamRootDocument(streamId, 1, DateTimeOffset.Now.Ticks);

//            var ret = JsonConvert.DeserializeObject<EventStreamModel>(JsonConvert.SerializeObject(root), JsonSettings);

//            await _es.AddAsync(root);

//            return ret;
//        }

//        public async Task<int> AddEventsToStreamAsync(string streamId, long expectedVersion, EventModel[] events, CancellationToken cancellationToken = default)
//        {
//            var stream = (EventStoreStreamRootDocument)(await _es.GetByIdAsync(streamId));

//            if (stream.Version != expectedVersion)
//                throw new ConcurrencyException($"Expected version {expectedVersion}, actual {stream.Version}");

//            var ts = DateTimeOffset.Now.Ticks;

//            var ret = await _es.AddAsync(events.Select((em, i) => new EventStoreEventDocument(streamId, i, ts, em))
//                .Cast<IEventStoreDocument>().ToArray()
//            );

//            stream.Version += events.Length;

//            await _es.UpdateAsync(stream);

//            return ret;
//        }

//        public Task DeleteStreamByIdAsync(string streamId, CancellationToken cancellationToken = default)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task CreateSnapshotOfStreamAsync(string streamId, long expectedVersion, SnapshotModel snapshot, bool deleteOlderSnapshots = false, CancellationToken cancellationToken = default)
//        {
//            var stream = (EventStoreStreamRootDocument)(await _es.GetByIdAsync(streamId));

//            if (stream.Version != expectedVersion)
//                throw new ConcurrencyException($"Expected version {expectedVersion}, actual {stream.Version}");

//            var ts = DateTimeOffset.Now.Ticks;
//            await _es.AddAsync(new EventStoreSnapshotDocument(streamId, ts, snapshot));
//        }

//        public Task DeleteSnapshotsForStreamByIdAsync(string streamId, ulong olderThanVersion, CancellationToken cancellationToken = default)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
