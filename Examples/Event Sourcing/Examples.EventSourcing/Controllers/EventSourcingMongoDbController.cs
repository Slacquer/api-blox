using APIBlox.NetCore.Contracts;
using Examples.AggregateModels;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Class EventSourcingController.
    ///     Implements the <see cref="MongoAggregate" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <inheritdoc />
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class EventSourcingMongoDbController : EventSourcingController<MongoAggregate>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventSourcingMongoDbController" /> class.
        /// </summary>
        /// <param name="svc">The SVC.</param>
        public EventSourcingMongoDbController(IEventStoreService<MongoAggregate> svc)
            : base(svc)
        {
        }
    }
}
