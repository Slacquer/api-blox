using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Models;
using APIBlox.NetCore.Options;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.Azure.Documents.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
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

            options.CollectionProperties.Add("DummyAggregate", new CosmosDbCollectionProperties { });
            var opt = Options.Create(options);

            var c = new DocumentClient(new Uri(options.Endpoint), options.Key);

            var repo = new CosmosDbRepository<DummyAggregate>(c, new CamelCaseSettings(), opt);

            IEventStoreService<DummyAggregate> svc = new EventStoreService<DummyAggregate>(repo);

            return svc;
        }

        private static IEventStoreService<DummyAggregate> GetMongoDbBackedEventStoreService()
        {
            BsonClassMap.RegisterClassMap<DummyAggregate>();
            BsonClassMap.RegisterClassMap<Child>();

            var ctx = new CollectionContext(new MongoDbOptions { CnnString = "mongodb://localhost:27017", DatabaseId = "testDb" });
            var repo = new MongoDbRepository<DummyAggregate>(ctx, new JsonSerializerSettings());

            IEventStoreService<DummyAggregate> svc = new EventStoreService<DummyAggregate>(repo);

            return svc;
        }

        private static IEventStoreService<DummyAggregate> GetRavenDbBackedEventStoreService()
        {
            var ctx = new StoreContext(new RavenDbOptions { DatabaseId = "testDb", Urls = new[] { "http://127.0.0.1:8080" } });
            var repo = new RavenDbRepository<DummyAggregate>(ctx, new JsonSerializerSettings());

            IEventStoreService<DummyAggregate> svc = new EventStoreService<DummyAggregate>(repo);
            return svc;
        }

        private static IEventStoreService<DummyAggregate> GetEfCoreSqlBackedEventStoreService()
        {
            var options = new DbContextOptionsBuilder<EventStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var ctx = new EventStoreDbContext(options);
            var repo = new EfCoreSqlRepository<DummyAggregate>(ctx, new JsonSerializerSettings());

            IEventStoreService<DummyAggregate> svc = new EventStoreService<DummyAggregate>(repo);
            return svc;
        }

        [Fact]
        public async Task CosmosDbFullTest()
        {
            var svc = GetCosmosbBackedEventStoreService();

            await RunCommon(svc);
        }

        [Fact]
        public async Task MongoDbFullTest()
        {
            var svc = GetMongoDbBackedEventStoreService();

            await RunCommon(svc);
        }

        [Fact]
        public async Task RavenDbFullTest()
        {
            var svc = GetRavenDbBackedEventStoreService();

            await RunCommon(svc);
        }

        [Fact]
        public async Task EfCoreSqlServerFullTest()
        {
            var svc = GetEfCoreSqlBackedEventStoreService();

            await RunCommon(svc);
        }

        private static async Task RunCommon(IEventStoreService<DummyAggregate> svc)
        {
            var agg = new DummyAggregate { StreamId = "test-doc" };

            await svc.DeleteEventStreamAsync(agg.StreamId);
            await Task.Delay(100);

            var lst = new List<EventModel> { new EventModel { Data = new { someTHing = 99 } }, new EventModel { Data = "2" }, new EventModel { Data = "3" } };

            var eventStoreDoc = await svc.WriteToEventStreamAsync(agg.StreamId, lst.ToArray());
            await Task.Delay(100);

            Assert.NotNull(eventStoreDoc);

            lst = new List<EventModel> { new EventModel { Data = "4" } };

            eventStoreDoc = await svc.WriteToEventStreamAsync(agg.StreamId, lst.ToArray(), 3);
            await Task.Delay(100);
            Assert.NotNull(eventStoreDoc);

            var result = await svc.ReadEventStreamAsync(agg.StreamId);
            await Task.Delay(100);

            Assert.NotNull(result);
            Assert.True(result.Version == 4);
            Assert.NotNull(result.StreamId);
            Assert.True(result.StreamId == agg.StreamId);
            Assert.Null(result.Snapshot);
            Assert.NotNull(result.Events);

            agg.Children = new List<Child>
            {
                new Child{ Foo="aaa", Bar=123},
                new Child{ Foo="bbb", Bar=456},
                new Child{ Foo="ccc", Bar=789},
            };
            await svc.CreateSnapshotAsync(result.StreamId, result.Version, new SnapshotModel { Data = agg });
            await Task.Delay(100);

            lst = new List<EventModel> { new EventModel { Data = "5" } };

            eventStoreDoc = await svc.WriteToEventStreamAsync(result.StreamId, lst.ToArray(), result.Version);
            await Task.Delay(100);

            Assert.NotNull(eventStoreDoc);

            result = await svc.ReadEventStreamAsync(agg.StreamId);
            await Task.Delay(100);

            Assert.True(result.Version == 5);
            Assert.True(result.Events.Length == 1, $"length is {result.Events.Length}");
            Assert.NotNull(result.Snapshot);
            Assert.NotNull(result.Snapshot.Data as DummyAggregate);

            var data = (DummyAggregate)result.Snapshot.Data;
            Assert.True(((List<Child>)data.Children)[0].Structure.Num1 == 44);

            await svc.CreateSnapshotAsync(result.StreamId, result.Version, new SnapshotModel { Data = "snapshot2" }, true);
            await Task.Delay(100);

            result = await svc.ReadEventStreamAsync(agg.StreamId);
            await Task.Delay(100);

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

        public IEnumerable<Child> Children { get; set; }
    }


    public class Child
    {
        public string Foo { get; set; }

        [JsonProperty(PropertyName = "fubar")]
        public int Bar { get; set; }

        [BsonIgnore]
        public MySstruct Structure { get; set; } = new MySstruct { Num1 = 44, Num2 = 12313 };
    }

    public struct MySstruct
    {
        public int Num1 { get; set; }
        public double Num2 { get; set; }
    }
}
