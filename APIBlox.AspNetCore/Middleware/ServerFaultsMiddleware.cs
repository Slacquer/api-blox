using System;
using System.Net;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Exceptions;
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
        private readonly bool _verboseProduction;

        public ServerFaultsMiddleware(
            RequestDelegate next,
            ILogger<ServerFaultsMiddleware> logger,
            IHostingEnvironment env,
            string typeUrl,
            bool verboseProduction,
            Func<string> referenceIdFunc
        )
        {
            _next = next;
            _log = logger;
            _env = env;
            _typeUrl = typeUrl;
            _verboseProduction = verboseProduction;
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
                string response;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Headers["Content-Type"] = InternalHelpers.ErrorResponseContentType;

                if (error.Error is HandledRequestException handled)
                {
                    if (handled.RequestErrorObject.Status != null)
                        context.Response.StatusCode = handled.RequestErrorObject.Status.Value;

                    response = handled.RequestErrorObject.Serialize();
                }
                else
                    response = BuildResponse(error.Error, context.Request.Path);

                await context.Response.WriteAsync(response, context.RequestAborted).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.LogCritical("Could not write response, Ex: {0}", ex.ToDynamicDataObject(true));

                if (!_env.IsProduction())
                    throw;
            }
        }

        private string BuildResponse(Exception err, string instance)
        {
            var serialized = $"Could not create NON production response.  Error {err.Message}";

            try
            {
                serialized = BuildNonProdResponse(err, instance);

                // We always want to log the full meal deal, however do not display it to user(s) when in production.
                _log.LogCritical(serialized);
            }
            catch (Exception ex)
            {
                _log.LogCritical(() => $"Could not create NON production response: {ex.Message}");
            }

            if (!_env.IsProduction() || _verboseProduction)
                return serialized;

            try
            {
                // What we show the consumer in production.
                serialized = BuildProdResponse(instance);
            }
            catch (Exception ex)
            {
                _log.LogCritical(() => $"Could not create production response: {ex.Message}");
            }

            return serialized;
        }

        private string BuildNonProdResponse(Exception err, string instance)
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

            return dto.Serialize();
        }

        private string BuildProdResponse(string instance)
        {
            var dto = new ServerErrorObject("An internal server error has occured.",
                "Please refer to the errors property for additional information.",
                (int)HttpStatusCode.InternalServerError,
                instance,
                _referenceIdFunc()
            )
            {
                NoThrow = true,
                Type = _typeUrl,
                Errors = null,
                Detail = null,
                Title = "Please contact support."
            };

            return dto.Serialize();
        }
    }
}
