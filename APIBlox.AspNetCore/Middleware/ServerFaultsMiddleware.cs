using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Extensions;
using APIBlox.AspNetCore.Types.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class ServerFaultsMiddleware
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<ServerFaultsMiddleware> _log;
        private readonly RequestDelegate _next;
        private readonly Func<string> _referenceIdFunc;
        private readonly string _typeUrl;

        public ServerFaultsMiddleware(
            RequestDelegate next,
            ILogger<ServerFaultsMiddleware> logger,
            IHostingEnvironment env,
            string typeUrl,
            Func<string> referenceIdFunc
        )
        {
            _next = next;
            _log = logger;
            _env = env;
            _typeUrl = typeUrl;
            _referenceIdFunc = referenceIdFunc ?? (() => DateTimeOffset.Now.Ticks.ToString());
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var error = context.Features.Get<IExceptionHandlerFeature>();

            if (error is null)
            {
                _log.LogInformation(() => "Skipping as no errors have occured.");

                await _next.Invoke(context).ConfigureAwait(false);

                return;
            }

            try
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Headers["Content-Type"] = "application/problem+json";

                await context.Response.WriteAsync(BuildResponse(error.Error, context.Request.Path),
                    context.RequestAborted
                ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.LogCritical("Could not write response, Ex: {0}", BuildError(ex, null));

                if (!_env.IsProduction())
                    throw;
            }
        }

        private string BuildResponse(Exception err, string instance)
        {
            var dto = new ServerErrorObject("An internal server error has occured.",
                "Please refer to the errors property for additional information.",
                (int)HttpStatusCode.InternalServerError,
                instance,
                _referenceIdFunc()
            )
            {
                Type = _typeUrl
            };

            dto.Errors.Add(err.ToDynamicDataObject());

            var serialized = dto.Serialize();

            // We always want to log the full meal deal, however do not display it to user(s) when in production.
            _log.LogCritical(serialized);

            if (!_env.IsProduction())
                return serialized;

            dto.NoThrow = true;
            dto.Errors = null;
            dto.Detail = null;
            dto.Title = "Please contact support.";

            serialized = dto.Serialize();

            _log.LogInformation(() => $"PRODUCTION Exception Message Result: {serialized}");

            return serialized;
        }

        private static string BuildError(Exception ex, StringBuilder sb)
        {
            sb = sb ?? new StringBuilder();

            sb.AppendLine($"{ex.GetType().Name}: {ex.Message}");


            if (ex.InnerException is null)
                return sb.ToString();

            BuildError(ex.InnerException, sb);

            return sb.ToString();
        }
    }
}
