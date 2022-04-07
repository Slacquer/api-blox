using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace APIBlox.AspNetCore.Filters
{
    // TODO: Doing it this way is lame, we need to be able to simply get a timestamp from data store.

    internal class ETagActionFilter : IAsyncActionFilter
    {
        private const int ActionMaxAgeDefault = 600; // client cache time 10min.
        private const int ActionSharedMaxAgeDefault = 86400; // caching proxy cache time  24hrs.

        private readonly ILogger<ETagActionFilter> _log;

        private readonly int _maxAgeSeconds;
        private readonly int _sharedMaxAgeSecond;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ETagActionFilter" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="maxAgeSeconds">The maximum age seconds.</param>
        /// <param name="sharedMaxAgeSecond">The shared maximum age second.</param>
        public ETagActionFilter(
            ILoggerFactory loggerFactory, int maxAgeSeconds = ActionMaxAgeDefault,
            int sharedMaxAgeSecond = ActionSharedMaxAgeDefault
        )
        {
            _log = loggerFactory.CreateLogger<ETagActionFilter>();
            _maxAgeSeconds = maxAgeSeconds;
            _sharedMaxAgeSecond = sharedMaxAgeSecond;
        }

        /// <inheritdoc />
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultCtx = await next().ConfigureAwait(false);
            var req = resultCtx.HttpContext.Request;
            var res = resultCtx.HttpContext.Response;

            if (!req.Method.EqualsEx("get")
                || res.StatusCode != StatusCodes.Status200OK
                || req.Headers[HeaderNames.IfNoneMatch] == StringValues.Empty
                || resultCtx.Result is not ObjectResult result)
            {
                _log.LogInformation(() =>
                    $"Skipping execute, Method: {req.Method}, StatusCode: {res.StatusCode}, " +
                    $"HeadersIfNonMatch? is empty? {req.Headers[HeaderNames.IfNoneMatch] == StringValues.Empty}, " +
                    $"result is objectResult? {resultCtx.Result is ObjectResult}"
                );

                return;
            }

            if (ShouldNotCache(res, out var cc))
                return;

            var etag = GenerateEtag(result);

            AddCacheControlAndETagHeaders(res, cc, etag);

            if (req.Headers[HeaderNames.IfNoneMatch].ToString().Replace("\"", "") != etag)
            {
                _log.LogInformation(() => $"Skipping execute, incoming eTag {etag} is the same.");

                return;
            }

            var p = req.Path;
            _log.LogInformation(() => $"Sending cached data for {p}");

            resultCtx.Result = new StatusCodeResult(304);
        }

        private static string GenerateEtag(IActionResult result)
        {
            var str = JsonConvert.SerializeObject(result);
            var arr = str.ToCharArray();
            var keyBytes = Encoding.UTF8.GetBytes(str);
            var bytes = new byte[keyBytes.Length + arr.Length];

            Buffer.BlockCopy(keyBytes, 0, bytes, 0, keyBytes.Length);
            Buffer.BlockCopy(arr, 0, bytes, keyBytes.Length, arr.Length);

            using var md5 = MD5.Create();

            var hash = md5.ComputeHash(bytes);
            var hex = BitConverter.ToString(hash);

            return hex.Replace("-", "");
        }

        private static bool ShouldNotCache(
            HttpResponse res,
            out CacheControlHeaderValue cacheControlHeader
        )
        {
            cacheControlHeader = res.GetTypedHeaders().CacheControl ?? new CacheControlHeaderValue();

            return cacheControlHeader.NoCache || cacheControlHeader.NoStore;
        }

        private void AddCacheControlAndETagHeaders(
            HttpResponse res,
            CacheControlHeaderValue cacheControlHeader, string eTag
        )
        {
            // RFC7232
            // https://tools.ietf.org/html/rfc7232#section-4.1
            // ...the server generating a 304 response MUST generate any of the following header 
            // fields that WOULD have been sent in a 200(OK) response to the same 
            // request: Cache-Control, Content-Location, Date, ETag, Expires, and Vary.
            // so we must set cache-control headers for 200s OR 304s...
            cacheControlHeader.MaxAge ??= TimeSpan.FromSeconds(_maxAgeSeconds);

            cacheControlHeader.SharedMaxAge ??= TimeSpan.FromSeconds(_sharedMaxAgeSecond);

            res.GetTypedHeaders().CacheControl = cacheControlHeader;
            res.Headers.Add(HeaderNames.ETag, eTag);
        }
    }
}
