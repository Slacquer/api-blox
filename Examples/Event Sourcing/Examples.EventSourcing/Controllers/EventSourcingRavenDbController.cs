using APIBlox.NetCore.Contracts;
using Examples.AggregateModels;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Class EventSourcingController.
    ///     Implements the <see cref="RavenAggregate" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <inheritdoc />
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class EventSourcingRavenDbController : EventSourcingController<RavenAggregate>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventSourcingRavenDbController" /> class.
        /// </summary>
        /// <param name="svc">The SVC.</param>
        public EventSourcingRavenDbController(IEventStoreService<RavenAggregate> svc)
            : base(svc)
        {
        }
    }
}
