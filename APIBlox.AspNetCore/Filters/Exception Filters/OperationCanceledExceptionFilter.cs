#region -    Using Statements    -

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class OperationCanceledExceptionFilter : IAsyncExceptionFilter
    {
        #region -    Fields    -

        private readonly ILogger _log;

        #endregion

        #region -    Constructors    -

        public OperationCanceledExceptionFilter(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<OperationCanceledExceptionFilter>();
        }

        #endregion

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
