
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using Examples.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Class EventSourcingController.
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class EventSourcingController : ControllerBase
    {
        private readonly IEventStoreService<MyAggregate> _es;

        public EventSourcingController(IEventStoreService<MyAggregate> eventStoreService)
        {
            _es = eventStoreService;
        }


        [HttpGet]
        public async Task<ActionResult> Get(string firstName)
        {
            var agg = new MyAggregate(_es, firstName);

            await agg.Build();

            return Ok(agg);
        }

        [HttpPost]
        public async Task<ActionResult> Post(MyAggregateResource resource, CancellationToken cancellationToken)
        {
            var agg = new MyAggregate(_es, resource.FirstName);
            
            await agg.AddSomeValue(resource.SomeValue, cancellationToken);
            await agg.PublishChangesAsync(cancellationToken);

            return Accepted();
        }

        [HttpPut("{firstName}")]
        public async Task<ActionResult> Put(string firstName,[FromBody] string someValue, CancellationToken cancellationToken)
        {
            var agg = new MyAggregate(_es, firstName);

            await agg.UpdateSomeValue(someValue, cancellationToken);
            await agg.PublishChangesAsync(cancellationToken);

            return Accepted();
        }
    }
}
