using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Exceptions;
using APIBlox.AspNetCore.Extensions;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Types;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class ServerFaultsMiddleware
    {
        private static readonly Regex StackTraceRegex = new Regex(@"([^\)]*\)) in (.*):line (\d)*$");
        private readonly IHostingEnvironment _env;
        private readonly ILogger<ServerFaultsMiddleware> _log;
        private readonly RequestDelegate _next;
        private readonly Func<string> _referenceIdFunc;
        private readonly string _typeUrl;
        private readonly bool _verboseProduction;
        private readonly bool _addUserStackTrace;
        private readonly Action<RequestErrorObject> _resultAction;

        public ServerFaultsMiddleware(
            RequestDelegate next,
            ILogger<ServerFaultsMiddleware> logger,
            IHostingEnvironment env,
            string typeUrl,
            bool verboseProduction,
            bool addUserStackTrace,
            Func<string> referenceIdFunc,
            Action<RequestErrorObject> requestErrorObjectAction
        )
        {
            _next = next;
            _log = logger;
            _env = env;
            _typeUrl = typeUrl;
            _verboseProduction = verboseProduction;
            _addUserStackTrace = addUserStackTrace;
            _referenceIdFunc = referenceIdFunc ?? (() => DateTimeOffset.Now.Ticks.ToString());
            _resultAction = requestErrorObjectAction;
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
                RequestErrorObject errorResult;
                var statusCode = (int)HttpStatusCode.InternalServerError;

                if (error.Error is HandledRequestException exception)
                {
                    var handled = exception;
                    errorResult = handled.RequestErrorObject;
                    statusCode = handled.RequestErrorObject.Status ?? (int)HttpStatusCode.InternalServerError;
                }
                else
                {
                    errorResult = BuildResponse(error.Error, context.Request.Path);
                }

                if (_addUserStackTrace)
                    AddUserStackTrace(errorResult, error.Error);

                _resultAction?.Invoke(errorResult);

                var result = new ProblemResult(errorResult)
                {
                    StatusCode = statusCode
                };

                await context.WriteResultExecutorAsync(result);
            }
            catch (InvalidOperationException iex)
            {
                if (iex.Message.Contains("No result executor"))
                    _log.LogCritical("It appears that you did not call services.AddServerFaults() during startup (ConfigureServices).");
                else
                    _log.LogCritical("Could not write response, Ex: {0}", iex.ToDynamicDataObject(true));

                if (!_env.IsProduction())
                    throw;
            }
            catch (Exception ex)
            {
                _log.LogCritical("Could not write response, Ex: {0}", ex.ToDynamicDataObject(true));

                if (!_env.IsProduction())
                    throw;
            }
        }

        private ServerErrorObject BuildResponse(Exception err, string instance)
        {
            ServerErrorObject ret = null;

            try
            {
                ret = BuildNonProdResponse(err, instance);

                // We always want to log the full meal deal,
                // however do not display it to user(s) when in production.
                _log.LogCritical(ret.Serialize());
            }
            catch (Exception ex)
            {
                _log.LogCritical(() => $"Could not create NON production response: {ex.Message}");
            }

            if (!_env.IsProduction() || _verboseProduction)
                return ret;

            try
            {
                // What we show the consumer in production.
                ret = BuildProdResponse(instance);
            }
            catch (Exception ex)
            {
                _log.LogCritical(() => $"Could not create production response: {ex.Message}");
            }

            return ret;
        }

        private ServerErrorObject BuildNonProdResponse(Exception err, string instance)
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

            return dto;
        }

        private ServerErrorObject BuildProdResponse(string instance)
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

            return dto;
        }

        private static void AddUserStackTrace(DynamicDataObject errResult, Exception ex)
        {
            var stackTraceLines = (ex.StackTrace ?? Environment.StackTrace)
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var sb = new StringBuilder();

            foreach (var ln in stackTraceLines)
            {
                if (!StackTraceRegex.IsMatch(ln))
                    continue;

                sb.AppendLine(ln);
            }

            if (errResult.HasProperty("Stack-trace"))
                errResult.RemoveProperty("Stack-trace");

            errResult.AddProperty("Stack-trace", sb.ToString());
        }
    }
}
