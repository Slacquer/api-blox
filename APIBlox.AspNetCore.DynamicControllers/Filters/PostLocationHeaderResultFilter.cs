﻿using System;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class PostLocationHeaderResultFilter : IAsyncResultFilter
    {
        private readonly ILogger<PostLocationHeaderResultFilter> _log;

        public PostLocationHeaderResultFilter(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<PostLocationHeaderResultFilter>();
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            context.HttpContext.Response.OnStarting(() =>
                {
                    try
                    {
                        HandleOnStarting(context);
                    }
                    catch (Exception ex)
                    {
                        _log.LogWarning(() => $"Could not generate post location header.  Ex: {ex.Message}");
                    }

                    return Task.CompletedTask;
                }
            );

            return next();
        }

        private static string FindId(ObjectResult result)
        {
            var jo = JObject.FromObject(result.Value);
            var id = jo.DescendantsAndSelf().OfType<JProperty>().FirstOrDefault(t => t.Name.EqualsEx("id"));

            if (id is null)
                throw new NullReferenceException(
                    "Response result does not appear to contain an " +
                    "ID property, therefore an GET url can not be created."
                );

            return id.Value.ToString();
        }

        private void HandleOnStarting(ResultExecutingContext context)
        {
            var req = context.HttpContext.Request;
            var res = context.HttpContext.Response;

            if (!req.Method.EqualsEx("post")
                || context.Controller is not ControllerBase controller
                || res.StatusCode != StatusCodes.Status201Created &&
                res.StatusCode != StatusCodes.Status200OK)
            {
                _log.LogInformation(() =>
                    $"Skipping execute, Method: {req.Method}, " +
                    $"Controller: {context.Controller.GetType()}, StatusCode: {res.StatusCode}."
                );

                return;
            }

            if (context.Result is not ObjectResult result)
                return;

            // ReSharper disable once Mvc.ActionNotResolved
            var partialUrl = $"{controller.Url.Action()}/{FindId(result)}";
            var uri = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? 80) {Path = partialUrl};
            var url = uri.Uri.AbsoluteUri;

            context.HttpContext.Response.Headers["Location"] = url;
            context.HttpContext.Response.StatusCode = StatusCodes.Status201Created;

            var endUri = url;

            if (endUri.Contains("?"))
                _log.LogWarning(() =>
                    "Something ain't jiving, perhaps you have missing some route " +
                    $"templates on your HttpMethodAttributes?  As url {endUri} " +
                    "should not contain an ID query param"
                );
            else
                _log.LogInformation(() =>
                    $"Set location header to: {url},"
                );
        }
    }
}
