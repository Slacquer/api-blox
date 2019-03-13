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
            var agg = new DummyAggregate { StreamId = "test-doc" };

            var options = new CosmosDbOptions
            {
                DatabaseId = "testDb",
                Endpoint = "https://localhost:8081",
                Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
            };

            options.Collections.Add("DummyAggregate", new Collection { Id = "dummy" });
            var opt = Options.Create(options);

            var c = new DocumentClient(new Uri(options.Endpoint), options.Key);

            var repo = new CosmosDbRepository<DummyAggregate>(c, opt);

            IEventStoreService<DummyAggregate> svc = new EventStoreService<DummyAggregate>(repo);

            var lst = new List<EventModel> { new EventModel { Data = "1" }, new EventModel { Data = "2" }, new EventModel { Data = "3" } };

            var eventStoreDoc = await svc.WriteToEventStreamAsync(agg.StreamId, lst.ToArray());

            Assert.NotNull(eventStoreDoc);

            lst = new List<EventModel> { new EventModel { Data = "4" } };

            eventStoreDoc = await svc.WriteToEventStreamAsync(agg.StreamId, lst.ToArray(), 3);
            Assert.NotNull(eventStoreDoc);

            var result = await svc.ReadEventStreamAsync(agg.StreamId, includeEvents: true);

            Assert.NotNull(result);
            Assert.True(result.Version == 4);
            Assert.NotNull(result.StreamId);
            Assert.True(result.StreamId == agg.StreamId);
            Assert.True(result.TimeStamp > 0);
            Assert.Null(result.Snapshot);
            Assert.NotNull(result.Events);

            await svc.CreateSnapshotAsync(result.StreamId, result.Version, new SnapshotModel { Data = "snapshot1" });

            lst = new List<EventModel> { new EventModel { Data = "5" } };

            eventStoreDoc = await svc.WriteToEventStreamAsync(result.StreamId, lst.ToArray(), result.Version);
            Assert.NotNull(eventStoreDoc);

            result = await svc.ReadEventStreamAsync(agg.StreamId, includeEvents: true);

            Assert.True(result.Version == 5);
            Assert.True(result.Events.Length == 1);
            Assert.NotNull(result.Snapshot);

            await svc.CreateSnapshotAsync(result.StreamId, result.Version, new SnapshotModel { Data = "snapshot12" }, deleteOlderSnapshots: true);


            await svc.DeleteEventStreamAsync(agg.StreamId);
        }
    }

    public class DummyAggregate //: IEventStoreDocument
    {
        public Guid AggregateId { get; } = Guid.NewGuid();


        public string SomeValue { get; set; }

        // I SHOULD NOT BE HERE!
        public string StreamId { get; set; }
    }

}
