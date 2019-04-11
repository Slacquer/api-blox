using APIBlox.NetCore.Contracts;
using Examples.AggregateModels;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Class EventSourcingController.
    ///     Implements the <see cref="CosmosAggregate" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <inheritdoc />
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class EventSourcingCosmosDbController : EventSourcingController<CosmosAggregate> // ControllerBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventSourcingCosmosDbController" /> class.
        /// </summary>
        /// <param name="svc">The SVC.</param>
        public EventSourcingCosmosDbController(IEventStoreService<CosmosAggregate> svc)
            : base(svc)
        {
        }
    }
}
