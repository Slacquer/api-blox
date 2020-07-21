
using APIBlox.AspNetCore.Attributes;
#if UseAPIBlox
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Enums;
using APIBlox.AspNetCore.Types;
using Examples.Contracts;
using Examples.Resources;
using Microsoft.AspNetCore.Mvc;

#endif

namespace Examples.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Class ExamplesController.
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
#if UseAPIBlox
    [Route("[environment]Api/versions/[version]/resources/[controller]")]
#else
    [Route("devApi/versions/1/resources/[controller]")]
#endif
    [ApiController]
    public class ExamplesController : ControllerBase
    {
        private readonly IRandomNumberGeneratorService _rndSvc;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExamplesController" /> class.
        /// </summary>
        /// <param name="randomNumberGeneratorService">The random number generator service.</param>
        public ExamplesController(IRandomNumberGeneratorService randomNumberGeneratorService)
        {
            _rndSvc = randomNumberGeneratorService;
        }

        /// <summary>
        ///     app.UseSimulateWaitTime(_environment); example, also shows pagination.
        /// </summary>
        /// <param name="query">Pagination options</param>
        /// <param name="wait">if not null then simulate wait middleware will kick in.</param>
        /// <returns>ActionResult&lt;IEnumerable&lt;System.String&gt;&gt;.</returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get([FromQuery] PaginationQuery query, string wait = null)
        {
            var examples = new List<string>();

            for (var i = 0; i < _rndSvc.GenerateNumber(1000); i++)
                examples.Add($"FuBar {i}");

            //var res = new HandlerResponse
            //{
            //    Result = new
            //    {
            //        Foo = new
            //        {
            //            Bar = new
            //            {
            //                Dummy = "Hello",
            //                ExampleResults = examples.Skip(query.Skip ?? 0).Take(query.Top ?? 10)
            //            }
            //        },
            //        SomeMetadata = "Some extra bits."

            //    }
            //};

            var res = new HandlerResponse
            {
                Result = examples.Skip(query.Skip ?? 0).Take(query.Top ?? 10)//.ToList()
            };

            return Ok(res.Result);
        }

        /// <summary>
        ///     ProblemResult example
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="description">The description, when empty no error details are displayed</param>
        /// <returns>ActionResult.</returns>
        [HttpGet("problemResult")]
#if UseAPIBlox
        public ActionResult GetProblemResultExample(CommonStatusCodes statusCode = CommonStatusCodes.Status403Forbidden, string description = null)
        {
            if (statusCode == CommonStatusCodes.Status200Ok || statusCode == CommonStatusCodes.Status204NoContent)
                return Ok("Try one that isn't a success code :)");

            var errObject = new RequestErrorObject();
            errObject.SetError(statusCode, description);

            return new ProblemResult(errObject);
#else
        public ActionResult GetErrorResponseExample(int statusCode, string description = null)
        {
            HttpContext.Response.StatusCode = statusCode;

            return new ObjectResult(description);
#endif
        }

        /// <summary>
        ///     app.UseServerFaults(); example
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
            throw new Exception("Be sure to try this out in RELEASE mode"

            //,
            //new IndexOutOfRangeException("As most if not all of this",
            //    new ArgumentException("error information is NOT displayed in production",
            //        new FileNotFoundException("By the way here is your message",
            //            new NullReferenceException(exceptionMessage)
            //        )
            //    )
            //)
            );
        }

        /// <summary>
        ///     services.AddPopulateRequestObjectActionFilter() example
        /// </summary>
        /// <param name="requestResource">The request resource.</param>
        [HttpPost("{valueId:int}/subResources")]
#if UseAPIBlox
        public ActionResult Post(ExampleRequestObject requestResource)
#else
        public ActionResult Post(ExampleRequestObject requestResource)
#endif
        {
            //
            //  SIDE NOTE:
            // we should be returning a route with id, but
            // I'm lazy and that's not the point of all this... :/

            return Conflict(new
            {
                detail = "Please see errors property for more details",
                errors = new[]
                    {
                        new
                        {
                            detail = "he userSettings does not exist for the supplied Id/Key.",
                            title = "The request method does not allow this functionality as upsert semantics are not supported."
                        }
                    }
            }
            );

            //  return Ok(new {Id = 1, requestResource.CoolNewValue, requestResource.ValueId});
        }

        /// <summary>
        ///     services.AddOperationCanceledExceptionFilter() example
        /// </summary>
        /// <remarks>
        ///     This method doesn't PUT anything, its actually an example that will show how the APIBlox
        ///     OperationCanceledExceptionFilter will prevent errors from showing up in your logs when someone makes a call to one
        ///     of your actions, doesn't bother to wait, and ends up going to YOUTUBE (or somewhere else).  If you don't know what
        ///     i mean, be sure to comment out the  startup entry services.AddOperationCanceledExceptionFilter().  Then execute
        ///     this action, and within 30 seconds browse to a new location.
        /// </remarks>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken"></param>
        [HttpPut("{id:int}")]
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
