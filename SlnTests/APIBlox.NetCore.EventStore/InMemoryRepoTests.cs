using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Documents;
using APIBlox.NetCore.Models;
using APIBlox.NetCore.Options;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace SlnTests.APIBlox.NetCore.EventStore
{
    public class InMemoryRepoTests
    {
        [Fact]
        public async Task ShouldBeAbleToCreateAndGetASingleItem()
        {
            var agg = new DummyAggregate();

            var options = new CosmosDbOptions
            {
                DatabaseId = "testDb",
                Endpoint = "https://localhost:8081",
                Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                BulkInsertFilePath = @".\bulkInsert.js"
            };

            options.Collections.Add("DummyAggregate", new Collection { Id = "dummy" });
            var opt = Options.Create(options);

            var c = new DocumentClient(new Uri(options.Endpoint), options.Key);

            var repo = new CosmosDbRepository<DummyAggregate>(c, opt);

            IEventStoreService<DummyAggregate> svc = new EventStoreService<DummyAggregate>(repo);

            var lst = new List<EventModel> { new EventModel("1"), new EventModel("2"), new EventModel("3") };

            var result = await svc.WriteToEventStreamAsync(agg.AggregateId.ToString(), lst.ToArray());

            //Assert.NotNull(result);
            //Assert.True(result.Version == 1);
            //Assert.NotNull(result.StreamId);
            //Assert.True(result.StreamId == agg.AggregateId.ToString());
            //Assert.True(result.TimeStamp > 0);
            //Assert.Null(result.Snapshot);
            //Assert.Null(result.Events);



            //var count = await svc.AddEventsToStreamAsync(result.StreamId, result.Version, lst.ToArray());

            //Assert.True(count == 3);

            //result = await svc.GetEventStreamAsync(agg.AggregateId.ToString());

            //Assert.True(result.Version == 4);

            //await svc.CreateSnapshotOfStreamAsync(result.StreamId, result.Version, new SnapshotModel("snapshot1"));



            //lst = new List<EventModel>();
            //lst.Add(new EventModel("4"));

            //count = await svc.AddEventsToStreamAsync(result.StreamId, result.Version, lst.ToArray());

            //result = await svc.GetEventStreamAsync(agg.AggregateId.ToString());

            //Assert.True(result.Version == 5);
            //Assert.NotNull(result.Snapshot);

        }
    }

    public class DummyAggregate : IEventStoreDocument
    {
        public Guid AggregateId { get; } = Guid.NewGuid();
        public string Id { get; }
        public long TimeStamp { get; }
    }

    public class InMemoryRepo<T> : IEventStoreRepository<T>
        where T : DocumentBase
    {
        private List<T> _items = new List<T>();

        public Task<int> AddAsync(params T[] eventObject)
        {
            _items.AddRange(eventObject);

            return Task.FromResult(eventObject.Length);
        }

        public Task<bool> DeleteAsync(Func<T, bool> predicate)
        {
            var rem = _items.Where(predicate).ToList();

            if (!rem.Any())
                return Task.FromResult(false);

            _items = _items.Except(rem).ToList();

            return Task.FromResult(true);
        }

        public Task<IEnumerable<T>> GetAsync(Func<T, bool> predicate)
        {
            var rem = _items.Where(predicate).ToList();

            if (!rem.Any())
                return null;

            return Task.FromResult<IEnumerable<T>>(rem);
        }

        public Task<T> GetByIdAsync(string id)
        {
            var existing = _items.FirstOrDefault(i => i.Id == id);

            return Task.FromResult(existing);
        }

        public Task UpdateAsync(T eventObject)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var itm = _items[i];

                if (itm.StreamId == eventObject.StreamId)
                {
                    _items[i] = eventObject;
                    break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
