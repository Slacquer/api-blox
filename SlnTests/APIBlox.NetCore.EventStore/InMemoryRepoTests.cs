//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using APIBlox.NetCore;
//using APIBlox.NetCore.Contracts;
//using APIBlox.NetCore.Models;
//using Xunit;

//namespace SlnTests.APIBlox.NetCore.EventStore
//{
//    public class InMemoryRepoTests
//    {
//        [Fact]
//        public async Task ShouldBeAbleToCreateAndGetASingleItem()
//        {
//            var agg = new DummyAggregate();
//            var repo = new InMemoryRepo<IEventStoreDocument>();

//            IEventStoreService<DummyAggregate> svc = new EventStoreService<DummyAggregate>(repo);

//            var result = await svc.AddStreamAsync(agg.AggregateId.ToString());

//            Assert.NotNull(result);
//            Assert.True(result.Version == 1);
//            Assert.NotNull(result.StreamId);
//            Assert.True(result.StreamId == agg.AggregateId.ToString());
//            Assert.True(result.TimeStamp > 0);
//            Assert.Null(result.Snapshot);
//            Assert.Null(result.Events);

//            var lst = new List<EventStoreEventDocument>();
//            lst.Add(new EventModel("1"));
//            lst.Add(new EventModel("2"));
//            lst.Add(new EventModel("3"));

//            var count = await svc.AddEventsToStreamAsync(result.StreamId, result.Version, lst.ToArray());

//            Assert.True(count == 3);
            
//            result = await svc.GetEventStreamAsync(agg.AggregateId.ToString());

//            Assert.True(result.Version == 4);

//            await svc.CreateSnapshotOfStreamAsync(result.StreamId, result.Version, new SnapshotModel("snapshot1"));



//            lst = new List<EventModel>();
//            lst.Add(new EventModel("4"));

//            count = await svc.AddEventsToStreamAsync(result.StreamId, result.Version, lst.ToArray());
            
//            result = await svc.GetEventStreamAsync(agg.AggregateId.ToString());

//            Assert.True(result.Version == 5);
//            Assert.NotNull(result.Snapshot);

//        }
//    }

//    public class DummyAggregate
//    {
//        public Guid AggregateId { get; } = Guid.NewGuid();
//    }

//    public class InMemoryRepo<T> : IEventStoreRepository<T>
//        where T : class, IEventStoreDocument
//    {
//        private List<T> _items = new List<T>();

//        public Task<int> AddAsync(params T[] eventObject)
//        {
//            _items.AddRange(eventObject);

//            return Task.FromResult(eventObject.Length);
//        }

//        public Task<bool> DeleteAsync(Func<T, bool> predicate)
//        {
//            var rem = _items.Where(predicate).ToList();

//            if (!rem.Any())
//                return Task.FromResult(false);

//            _items = _items.Except(rem).ToList();

//            return Task.FromResult(true);
//        }

//        public Task<IEnumerable<T>> GetAsync(Func<T, bool> predicate)
//        {
//            var rem = _items.Where(predicate).ToList();

//            if (!rem.Any())
//                return null;

//            return Task.FromResult<IEnumerable<T>>(rem);
//        }

//        public Task<T> GetByIdAsync(string streamId)
//        {
//            var existing = _items.FirstOrDefault(i => i.StreamId == streamId);

//            return Task.FromResult(existing);
//        }

//        public Task UpdateAsync(T eventObject)
//        {
//            for (int i = 0; i < _items.Count; i++)
//            {
//                var itm = _items[i];

//                if (itm.StreamId == eventObject.StreamId)
//                {
//                    _items[i] = eventObject;
//                    break;
//                }
//            }

//            return Task.CompletedTask;
//        }
//    }
//}
