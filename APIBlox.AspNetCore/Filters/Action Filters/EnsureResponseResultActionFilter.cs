using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace APIBlox.AspNetCore.Filters
{
    internal class EnsureResponseResultActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<EnsureResponseResultActionFilter> _log;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Filters.EnsureResponseResultActionFilter" />
        ///     class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>

        // ReSharper disable once MemberCanBeProtected.Global
        public EnsureResponseResultActionFilter(
            ILoggerFactory loggerFactory
        )
        {
            _log = loggerFactory.CreateLogger<EnsureResponseResultActionFilter>();
        }

        protected bool ResultValueIsEnumerable { get; private set; }

        protected int? ResultValueCount { get; private set; }

        public virtual async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var action = await next().ConfigureAwait(false);
            var result = action.Result as ObjectResult;

            if (result?.Value is null || !(result.StatusCode is null) 
                && (result.StatusCode != StatusCodes.Status200OK || !InternalHelpers.ApplyEnsureResponseCompliesWithQueryActionsOnly))
            {
                var sc = result != null
                    ? result.StatusCode
                    : action.HttpContext.Response.StatusCode;

                if (sc != 200)
                    _log.LogInformation(() => $"Skipping execute as StatusCode is not 200, StatusCode is: {sc}");

                return;
            }

            var t = result.Value.GetType();

            ResultValueIsEnumerable = t.IsAssignableTo(typeof(IEnumerable)) && !t.IsAssignableTo(typeof(string));
            ResultValueCount = ResultValueIsEnumerable ? ((IEnumerable<object>)result.Value).Count() : 0;

            var retValue = InternalHelpers.EnsureResponseCompliesWithAction(result.Value);

            if (!(retValue is null))
                result.Value = retValue;

            Handle(context, result);
        }

        protected virtual void Handle(ActionExecutingContext context, ObjectResult result)
        {
        }
    }
}
