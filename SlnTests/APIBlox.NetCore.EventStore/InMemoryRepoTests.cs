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
                Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                BulkInsertFilePath = @".\bulkInsert.js"
            };

            options.Collections.Add("DummyAggregate", new Collection { Id = "dummy" });
            var opt = Options.Create(options);

            var c = new DocumentClient(new Uri(options.Endpoint), options.Key);

            var repo = new CosmosDbRepository<DummyAggregate>(c, opt);

            IEventStoreService<DummyAggregate> svc = new EventStoreService<DummyAggregate>(repo);

            var lst = new List<EventModel> { new EventModel("1"), new EventModel("2"), new EventModel("3") };

            await svc.WriteToEventStreamAsync(agg.StreamId, lst.ToArray());

            lst = new List<EventModel> { new EventModel("4") };

            await svc.WriteToEventStreamAsync(agg.StreamId, lst.ToArray(), 3);

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

        public string StreamId { get; set; }
        public long TimeStamp { get; }
    }

}
