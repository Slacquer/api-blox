using APIBlox.NetCore.Contracts;
using Examples.AggregateModels;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Class EventSourcingController.
    ///     Implements the <see cref="EfCoreSqlAggregate" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <inheritdoc />
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class EventSourcingEfCoreController : EventSourcingController<EfCoreSqlAggregate>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventSourcingEfCoreController" /> class.
        /// </summary>
        /// <param name="svc">The SVC.</param>
        public EventSourcingEfCoreController(IEventStoreService<EfCoreSqlAggregate> svc)
            : base(svc)
        {
        }
    }
}
