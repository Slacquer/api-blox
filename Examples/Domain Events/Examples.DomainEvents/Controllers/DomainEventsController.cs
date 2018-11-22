using System.Collections.Generic;
using Examples.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    [Route("api/resources/[controller]")]
    [ApiController]
    public class DomainEventsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return Ok();
        }

        [HttpPost]
        public ActionResult Post(ExampleRequestObject requestResource)
        {
            return Ok();
        }
    }
}
