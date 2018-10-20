using System.Collections.Generic;
using System.Diagnostics;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Enums;
using APIBlox.AspNetCore.Extensions;
using APIBlox.AspNetCore.Types.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values        
        /// <summary>
        /// Gets the specified wait.
        /// </summary>
        /// <param name="wait">if set to <c>true</c> [wait].</param>
        /// <returns>ActionResult&lt;IEnumerable&lt;System.String&gt;&gt;.</returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get(bool wait = true)
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("problemResult")]
        public ActionResult GetErrorResponseExample(CommonStatusCodes statusCode, string description)
        {
            if (statusCode == CommonStatusCodes.Ok)
                return Ok("Try one that isn't a success code :)");

            var errObject = new RequestErrorObject();
            errObject.SetError(statusCode, description);

            return new ProblemResult(errObject);
        }

        [DebuggerStepThrough]
        [HttpGet("serverFault")]
        public ActionResult ThrowExceptionForServerFaultExample(string exceptionMessage)
        {
            throw new System.Exception("Be sure to try this out in RELEASE mode",
                new System.IndexOutOfRangeException("As most if not all of this",
                    new System.ArgumentException("error information is NOT displayed in production",
                        new System.IO.FileNotFoundException("By the way here is your message",
                            new System.NullReferenceException(exceptionMessage)
                        )
                    )
                )
            );
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
