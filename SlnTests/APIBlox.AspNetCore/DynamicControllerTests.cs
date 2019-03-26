using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APIBlox.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace SlnTests.APIBlox.AspNetCore
{
    public class DynamicControllerTests
    {
        [Fact]
        public void Foo()
        {
            var contents = @"
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SomeNameSpace.Controllers
{
    [Route(""api/[controller]"")]
    [ApiController]
    public sealed class @GetController : ControllerBase
    {
        private readonly IQueryHandler<@RequestObj, HandlerResponse> _getAllHandler;

        public @GetController(IQueryHandler<@RequestObj, HandlerResponse> getAllHandler)
        {
            _getAllHandler = getAllHandler;
        }

        /// <summary>
        ///     Action for getting a collection of resources.
        ///     <para>
        ///         Responses: 200, 204, 401, 403
        ///     </para>
        /// </summary>
        /// <param name=""request"">@RequestObj input parameter(s)</param>
        /// <param name=""cancellationToken"">The cancellation token.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<@ResponseObj>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll(@params, CancellationToken cancellationToken)
        {
            var obj = new @RequestObj();
            var ret = await _getAllHandler.HandleAsync(obj, cancellationToken).ConfigureAwait(false);

            if (ret.HasErrors)
                return new ProblemResult(ret.Error);

            return ret.Result is null ? NoContent() : (IActionResult) Ok(ret.Result);
        }
    }
}
";

            var controllerName = "MyController";
            var requestObjectType = "SlnTests.APIBlox.AspNetCore.MyControllerParams";
            var responseObjectType = "long";
            var props = typeof(MyControllerParams).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).ToList();

            var parameters = new StringBuilder();

            for (var index = 0; index < props.Count; index++)
            {
                var pi = props[index];

                var v = index < props.Count-1 ? ", " : "";
                parameters.Append($"{pi.DeclaringType.FullName} {pi.Name}{v}");
            }

            contents = contents.Replace("@GetController", controllerName)
            .Replace("@RequestObj", requestObjectType)
            .Replace("@ResponseObj", responseObjectType)
            .Replace("@params", parameters.ToString());

            var types = contents.ToTypes(out var foo);

            Assert.NotNull(types);
            Assert.Null(foo);

        }
    }

    public class MyControllerParams
    {
        [FromRoute(Name ="id")]
        public int Id { get; set; }

        [FromBody] public MyBody Body { get; set; }
    }

    public class MyBody
    {
        public string Foo { get; set; }
        public string Bar { get; set; }
    }
}
