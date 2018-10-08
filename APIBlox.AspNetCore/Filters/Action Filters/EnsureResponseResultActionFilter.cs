#region -    Using Statements    -

using System;
using System.Collections;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore.Filters
{
    internal class EnsureResponseResultActionFilter : IAsyncActionFilter
    {
        #region -    Fields    -

        private readonly Func<object, object> _defineResponseFunc;
        private readonly ILogger<EnsureResponseResultActionFilter> _log;

        #endregion

        #region -    Constructors    -

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Filters.EnsureResponseResultActionFilter" />
        ///     class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="compliance">Compliance Response</param>

        // ReSharper disable once MemberCanBeProtected.Global
        public EnsureResponseResultActionFilter(
            ILoggerFactory loggerFactory,
            IEnsureResponseCompliesWith compliance
        )
        {
            _log = loggerFactory.CreateLogger<EnsureResponseResultActionFilter>();
            _defineResponseFunc = compliance.Func;
        }

        #endregion

        protected bool ResultValueIsEnumerable { get; private set; }

        public virtual async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var action = await next().ConfigureAwait(false);
            var result = action.Result as ObjectResult;

            if (result?.Value is null || !(result.StatusCode is null) && result.StatusCode != StatusCodes.Status200OK)
            {
                var sc = result != null
                    ? result.StatusCode
                    : action.HttpContext.Response.StatusCode;

                if (sc != 200)
                    _log.LogInformation(() => $"Skipping execute as StatusCode is not 200, StatusCode is: {sc}");
                
                return;
            }

            var t = result.Value.GetType();
            ResultValueIsEnumerable = t.IsAssignableTo(typeof(IEnumerable));
            var retValue = _defineResponseFunc(result.Value);

            if (!(retValue is null))
                result.Value = retValue;

            Handle(result);
        }

        protected virtual void Handle(ObjectResult result)
        {
        }
    }
}
