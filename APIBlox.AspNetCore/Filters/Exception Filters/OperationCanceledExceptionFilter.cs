using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class OperationCanceledExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger _log;

        public OperationCanceledExceptionFilter(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<OperationCanceledExceptionFilter>();
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (!(context.Exception is OperationCanceledException))
            {
                _log.LogInformation(() => $"Skipping execute, exception is of type {context.Exception.GetType()}");

                return Task.CompletedTask;
            }

            _log.LogInformation(() => $"Request {context.HttpContext.Request.Path} was cancelled.");

            context.ExceptionHandled = true;

            // Client Closed Request, this is a non-standard status code used by NGINX
            context.Result = new StatusCodeResult(499);

            return Task.CompletedTask;
        }
    }
}
