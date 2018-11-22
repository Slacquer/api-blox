using Examples.Contracts;
using Examples.DomainModels;
using Examples.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     NON queued domain events controller.
    ///     <para>
    ///         All events are created INSIDE the entities themselves.  Personally,  I use 2
    ///         different types of entities, for example IDomainModel and/or IDomainModelWithEvents.
    ///         The reason should be clear, you will not always use domain events and
    ///         I don't think the entities should be unnecessarily bloated.
    ///     </para>
    /// </summary>
    [Route("api/resources/[controller]")]
    [ApiController]
    public class NonQueuedDomainEventsController : ControllerBase
    {
        private readonly ILameRepository _repository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NonQueuedDomainEventsController" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public NonQueuedDomainEventsController(ILameRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        ///     Creates an aggregate root entity named DomainObject
        ///     internally it adds the new event to its own internal collection.
        ///     Upon saving changes the repository will then inform the event
        ///     dispatcher of all the events in all of the models in its collection.
        ///     For me, this was an easy concept to understand, however mastering
        ///     it will be much more difficult... as there are several ways to do this.
        ///     Be sure to do your research.  What I have created in the 2 different dispatchers
        ///     should allow you to handle your events any way you see fit.
        /// </summary>
        /// <param name="requestResource">The request resource.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public ActionResult Post(ExampleRequestObject requestResource)
        {
            var model = new DomainObject(requestResource.SomeValue);

            _repository.AddDomainObject(model);

            _repository.SaveChanges();

            return Ok();
        }
    }
}
