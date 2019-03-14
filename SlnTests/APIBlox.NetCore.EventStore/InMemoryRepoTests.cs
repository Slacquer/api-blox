using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.EventStore;
using APIBlox.NetCore.EventStore.Options;
using APIBlox.NetCore.Models;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace SlnTests.APIBlox.NetCore.EventStore
{
    public class InMemoryRepoTests
    {
        private static IEventStoreService<DummyAggregate> GetCosmosbBackedEventStoreService()
        {
            var options = new CosmosDbOptions
            {
                DatabaseId = "testDb",
                Endpoint = "https://localhost:8081",
                Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
            };

            //options.Collections.Add("DummyAggregate", new Collection { Id = "dummy" });
            var opt = Options.Create(options);

            var c = new DocumentClient(new Uri(options.Endpoint), options.Key);

            var repo = new CosmosDbRepository<DummyAggregate>(c, new CamelCaseSettings(), opt);

            IEventStoreService<DummyAggregate> svc = new EventStoreService<DummyAggregate>(repo);

            return svc;
        }

        private static IEventStoreService<DummyAggregate> GetMongoDbBackedEventStoreService()
        {
            var ctx = new CollectionContext("mongodb://localhost:27017","testDb");
            var repo = new MongoDbRepository<DummyAggregate>(ctx, new JsonSerializerSettings());

            IEventStoreService<DummyAggregate> svc = new EventStoreService<DummyAggregate>(repo);
            return svc;
        }

        [Fact]
        public async Task CosmosDbFullTest()
        {
            var svc = GetCosmosbBackedEventStoreService();

            var agg = new DummyAggregate {StreamId = "test-doc"};

            await svc.DeleteEventStreamAsync(agg.StreamId);

            var lst = new List<EventModel> {new EventModel {Data = "1"}, new EventModel {Data = "2"}, new EventModel {Data = "3"}};

            var eventStoreDoc = await svc.WriteToEventStreamAsync(agg.StreamId, lst.ToArray());

            Assert.NotNull(eventStoreDoc);

            lst = new List<EventModel> {new EventModel {Data = "4"}};

            eventStoreDoc = await svc.WriteToEventStreamAsync(agg.StreamId, lst.ToArray(), 3);
            Assert.NotNull(eventStoreDoc);

            var result = await svc.ReadEventStreamAsync(agg.StreamId, includeEvents: true);

            Assert.NotNull(result);
            Assert.True(result.Version == 4);
            Assert.NotNull(result.StreamId);
            Assert.True(result.StreamId == agg.StreamId);
            Assert.Null(result.Snapshot);
            Assert.NotNull(result.Events);

            await svc.CreateSnapshotAsync(result.StreamId, result.Version, new SnapshotModel {Data = "snapshot1"});

            lst = new List<EventModel> {new EventModel {Data = "5"}};

            eventStoreDoc = await svc.WriteToEventStreamAsync(result.StreamId, lst.ToArray(), result.Version);
            Assert.NotNull(eventStoreDoc);

            result = await svc.ReadEventStreamAsync(agg.StreamId, includeEvents: true);

            Assert.True(result.Version == 5);
            Assert.True(result.Events.Length == 1);
            Assert.NotNull(result.Snapshot);

            await svc.CreateSnapshotAsync(result.StreamId, result.Version, new SnapshotModel {Data = "snapshot2"}, true);

            result = await svc.ReadEventStreamAsync(agg.StreamId, includeEvents: true);

            Assert.True(result.Version == 5);
            Assert.True(result.Events.Length == 0);
            Assert.NotNull(result.Snapshot);
            Assert.True(result.Snapshot.Data.Equals("snapshot2"));
        }

        [Fact]
        public async Task MongoDbFullTest()
        {
            var svc = GetMongoDbBackedEventStoreService();

            var agg = new DummyAggregate {StreamId = "test-doc"};

            await svc.DeleteEventStreamAsync(agg.StreamId);

            var lst = new List<EventModel> {new EventModel {Data = "1"}, new EventModel {Data = "2"}, new EventModel {Data = "3"}};

            var eventStoreDoc = await svc.WriteToEventStreamAsync(agg.StreamId, lst.ToArray());

            Assert.NotNull(eventStoreDoc);

            lst = new List<EventModel> {new EventModel {Data = "4"}};

            eventStoreDoc = await svc.WriteToEventStreamAsync(agg.StreamId, lst.ToArray(), 3);
            Assert.NotNull(eventStoreDoc);

            var result = await svc.ReadEventStreamAsync(agg.StreamId, includeEvents: true);

            Assert.NotNull(result);
            Assert.True(result.Version == 4);
            Assert.NotNull(result.StreamId);
            Assert.True(result.StreamId == agg.StreamId);
            Assert.Null(result.Snapshot);
            Assert.NotNull(result.Events);

            await svc.CreateSnapshotAsync(result.StreamId, result.Version, new SnapshotModel {Data = "snapshot1"});

            lst = new List<EventModel> {new EventModel {Data = "5"}};

            eventStoreDoc = await svc.WriteToEventStreamAsync(result.StreamId, lst.ToArray(), result.Version);
            Assert.NotNull(eventStoreDoc);

            result = await svc.ReadEventStreamAsync(agg.StreamId, includeEvents: true);

            Assert.True(result.Version == 5);
            Assert.True(result.Events.Length == 1);
            Assert.NotNull(result.Snapshot);

            await svc.CreateSnapshotAsync(result.StreamId, result.Version, new SnapshotModel {Data = "snapshot2"}, false);

            result = await svc.ReadEventStreamAsync(agg.StreamId, includeEvents: true);

            Assert.True(result.Version == 5);
            Assert.True(result.Events.Length == 0);
            Assert.NotNull(result.Snapshot);
            Assert.True(result.Snapshot.Data.Equals("snapshot2"));
        }
    }

    public class DummyAggregate 
    {
        public Guid AggregateId { get; } = Guid.NewGuid();

        public string SomeValue { get; set; }

        // I SHOULD NOT BE HERE!
        public string StreamId { get; set; }
    }
}
