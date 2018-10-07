﻿#region -    Using Statements    -

using System;
using System.Net;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Errors;
using APIBlox.AspNetCore.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class ServerFaultsMiddleware
    {
        #region -    Fields    -

        private readonly IHostingEnvironment _env;
        private readonly ILogger<ServerFaultsMiddleware> _log;
        private readonly RequestDelegate _next;
        private readonly Func<string> _referenceIdFunc;
        private readonly string _typeUrl;

        #endregion

        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerFaultsMiddleware" /> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="env">The env.</param>
        /// <param name="typeUrl">The url to specify in the <see cref="ProblemDetails.Type" /></param>
        /// <param name="referenceIdFunc">
        ///     A callback function to set the reference number of the fault, by default this is
        ///     <see cref="DateTimeOffset.Now" />.Ticks
        /// </param>
        public ServerFaultsMiddleware(
            RequestDelegate next, ILogger<ServerFaultsMiddleware> logger,
            IHostingEnvironment env, string typeUrl, Func<string> referenceIdFunc
        )
        {
            _next = next;
            _log = logger;
            _env = env;
            _typeUrl = typeUrl;
            _referenceIdFunc = referenceIdFunc ?? (() => DateTimeOffset.Now.Ticks.ToString());
        }

        #endregion

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
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.Headers["Content-Type"] = "application/problem+json";

                await context.Response.WriteAsync(
                    BuildResponse(error.Error, context.Request.Path),
                    context.RequestAborted
                ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.LogCritical("Could not write response, Ex: {0}", ex.Message);

                throw;
            }
        }

        private string BuildResponse(Exception err, string instance)
        {
            var dto = new ServerErrorObject(
                "An internal server error has occured.",
                "Please refer to the errors property for additional information.",
                (int) HttpStatusCode.InternalServerError,
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

            dto.Errors = null;
            dto.Detail = null;
            dto.Title = "Please contact support.";

            return dto.Serialize();
        }
    }
}
