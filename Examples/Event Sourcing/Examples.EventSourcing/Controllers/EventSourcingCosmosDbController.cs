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
    public class EventSourcingCosmosDb2Controller : EventSourcingController<CosmosAggregate2> // ControllerBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventSourcingCosmosDb2Controller" /> class.
        /// </summary>
        /// <param name="svc">The SVC.</param>
        public EventSourcingCosmosDb2Controller(IEventStoreService<CosmosAggregate2> svc)
            : base(svc)
        {
        }
    }
}
