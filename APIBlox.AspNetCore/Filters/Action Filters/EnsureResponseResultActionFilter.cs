using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace APIBlox.AspNetCore.Filters
{
    internal class EnsureResponseResultActionFilter : IAsyncActionFilter
    {
        private readonly bool _getsOnly;
        private readonly Func<object, object> _action;
        private readonly ILogger<EnsureResponseResultActionFilter> _log;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnsureResponseResultActionFilter" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="getsOnly">if set to <c>true</c> [gets only].</param>
        /// <param name="ensureResponseCompliesWithAction">The ensure response complies with action.</param>
        public EnsureResponseResultActionFilter(
            ILoggerFactory loggerFactory,
            bool getsOnly,
            Func<object, object> ensureResponseCompliesWithAction
        )
        {
            static object DefaultFormat(object d)
            {
                return new { Data = d };
            }

            _getsOnly = getsOnly;
            _action = ensureResponseCompliesWithAction ?? DefaultFormat;
            _log = loggerFactory.CreateLogger<EnsureResponseResultActionFilter>();
        }

        protected bool ResultValueIsEnumerable { get; private set; }

        protected int? ResultValueCount { get; private set; }

        public virtual async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var action = await next().ConfigureAwait(false);
            var result = action.Result as ObjectResult;

            if (result?.Value is null
                || _getsOnly && !action.HttpContext.Request.Method.EqualsEx("get")
                || result.StatusCode >= 300
                || result.StatusCode < 200)
            {
                var sc = result != null
                    ? result.StatusCode
                    : action.HttpContext.Response.StatusCode;

                if (sc >= 300 || sc < 200)
                    _log.LogInformation(() => $"Skipping execute as StatusCode is: {sc}");

                return;
            }

            var t = result.Value.GetType();

            ResultValueIsEnumerable = t.IsAssignableTo(typeof(IEnumerable)) && !t.IsAssignableTo(typeof(string));
            ResultValueCount = ResultValueIsEnumerable ? ((IEnumerable<object>)result.Value).Count() : 0;

            if (!ResultValueIsEnumerable)
            {
                var found = FindFirstArrayItem(result.Value);

                if (found is not null)
                {
                    ResultValueIsEnumerable = true;
                    ResultValueCount = found.Count;
                }
            }

            var retValue = _action(result.Value);

            if (retValue is not null)
                result.Value = retValue;

            Handle(context, result);
        }

        protected virtual void Handle(ActionExecutingContext context, ObjectResult result)
        {
        }

        private static JArray FindFirstArrayItem(object value)
        {
            var jt = JToken.FromObject(value);

            if (jt is not JObject jo)
                return null;

            foreach (var property in jo.Properties())
            {
                if (property.Value is JArray ar)
                    return ar;

                var ret = FindFirstArrayItem(property.Value);

                if (ret is not null)
                    return ret;
            }

            return null;
        }
    }
}
