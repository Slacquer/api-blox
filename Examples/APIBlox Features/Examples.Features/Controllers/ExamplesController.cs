using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Attributes;
using APIBlox.AspNetCore.Enums;
using APIBlox.AspNetCore.Extensions;
using APIBlox.AspNetCore.Types.Errors;
using Examples.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Controllers
{
    /// <summary>
    ///     Class ExamplesController.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("[environment]Api/versions/[version]/resources/[controller]")]
    [ApiController]
    public class ExamplesController : ControllerBase
    {
        ///  <summary>
        ///      Gets the specified wait.
        ///  </summary>
        /// <param name="wait">if not null then simulate wait middleware will kick in.</param>
        /// 
        ///  <returns>ActionResult&lt;IEnumerable&lt;System.String&gt;&gt;.</returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get(string wait = null)
        {
            var examples = new List<string>();

            for (var i = 0; i < 24; i++)
                examples.Add($"FuBar {i}");

            return Ok(examples);
        }

        /// <summary>
        ///     Gets the error response example.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="description">The description, when empty no error details are displayed</param>
        /// <returns>ActionResult.</returns>
        [HttpGet("problemResult")]
        public ActionResult GetErrorResponseExample(CommonStatusCodes statusCode = CommonStatusCodes.Forbidden, string description = null)
        {
            if (statusCode == CommonStatusCodes.Ok)
                return Ok("Try one that isn't a success code :)");

            var errObject = new RequestErrorObject();
            errObject.SetError(statusCode, description);

            return new ProblemResult(errObject);
        }

        /// <summary>
        ///     Throws the exception for server fault example.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <returns>ActionResult.</returns>
        /// <exception cref="System.Exception">
        ///     Be sure to try this out in RELEASE mode - new System.IndexOutOfRangeException("As most if not all of this",
        ///     new System.ArgumentException("error information is NOT displayed in production",
        ///     new System.IO.FileNotFoundException("By the way here is your message",
        ///     new System.NullReferenceException(exceptionMessage)
        ///     )
        ///     )
        ///     )
        /// </exception>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     As most if not all of this - new System.ArgumentException("error information is NOT displayed in production",
        ///     new System.IO.FileNotFoundException("By the way here is your message",
        ///     new System.NullReferenceException(exceptionMessage)
        ///     )
        ///     )
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     error information is NOT displayed in production - new System.IO.FileNotFoundException("By the way here is your
        ///     message",
        ///     new System.NullReferenceException(exceptionMessage)
        ///     )
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">By the way here is your message</exception>
        /// <exception cref="System.NullReferenceException"></exception>
        [DebuggerStepThrough]
        [HttpGet("serverFault")]
        public ActionResult ThrowExceptionForServerFaultExample(string exceptionMessage)
        {
            throw new Exception("Be sure to try this out in RELEASE mode",
                new IndexOutOfRangeException("As most if not all of this",
                    new ArgumentException("error information is NOT displayed in production",
                        new FileNotFoundException("By the way here is your message",
                            new NullReferenceException(exceptionMessage)
                        )
                    )
                )
            );
        }

        /// <summary>
        ///     Posts the specified resource.
        /// </summary>
        /// <param name="requestResource">The request resource.</param>
        [HttpPost("{valueId}/subResources")]
        public void Post([Populate] ExampleRequestObject requestResource)
        {
            // You may be thinking... "Why would this be helpful, I mean I could just add
            // the parameters to the method and they get filled in for me!"
            //
            // Imagine a dynamic post controller, that could potentially handle just about
            // ANY post, have a look at the Dynamic Controllers example then you will
            // understand just how powerful this can be.
            //
            //  SIDE NOTE:
            // we should be returning a route with id, but
            // I'm lazy and that's not the point of all this... :/
        }

        /// <summary>
        ///     This method doesn't PUT anything, its actually an example that will show how
        ///     the APIBlox OperationCanceledExceptionFilter will prevent errors from showing up in your
        ///     logs when someone makes a call to one of your actions, doesn't bother to wait, and ends
        ///     up going to YOUTUBE (or somewhere else).  If you don't know what i mean, be sure to comment
        ///     out the  startup entry services.AddOperationCanceledExceptionFilter().  Then execute this
        ///     action, and within 30 seconds browse to a new location.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken"></param>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] string value, CancellationToken cancellationToken)
        {
            await Task.Delay(30000, cancellationToken);

            return NoContent();
        }

        /// <summary>
        ///     Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
