#region -    Using Statements    -

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class SimulateWaitTimeMiddleware
    {
        #region -    Fields    -

        private readonly List<string> _excludeUrlsLike;
        private readonly ILogger<SimulateWaitTimeMiddleware> _log;
        private readonly RequestDelegate _next;
        private readonly Random _rnd = new Random((int) DateTime.Now.Ticks);

        #endregion

        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="SimulateWaitTimeMiddleware" /> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="logger">Logger</param>
        /// <param name="excludeUrlsLike">The exclude urls like.</param>
        public SimulateWaitTimeMiddleware(
            RequestDelegate next,
            ILogger<SimulateWaitTimeMiddleware> logger,
            IEnumerable<string> excludeUrlsLike
        )
        {
            _log = logger;
            _next = next;
            _excludeUrlsLike = excludeUrlsLike.ToList();
        }

        #endregion

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value.ToLower();

            if (!context.Request.Query.ContainsKey("wait"))
            {
                _log.LogInformation(() => "Skipping as QUERY string does not contain WAIT.");
                await _next(context).ConfigureAwait(false);

                return;
            }

            if (_excludeUrlsLike.Any(u => path.Contains(u)))
            {
                _log.LogInformation(() =>
                    $"Skipping as request path {path} is " +
                    $"in the exclusion lis {string.Join(",", _excludeUrlsLike)}"
                );

                await _next(context).ConfigureAwait(false);

                return;
            }

            await GoToSleepAsync(path, context.RequestAborted).ConfigureAwait(false);
            await _next(context).ConfigureAwait(false);
        }

        private Task GoToSleepAsync(string path, CancellationToken token)
        {
            var delay = _rnd.Next(250, 5000);
            _log.LogInformation(() => $"Simulating wait for {delay}ms.  Request: {path}");

            return Task.Delay(delay, token);
        }
    }
}
