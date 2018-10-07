#region -    Using Statements    -

using System;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class PostLocationHeaderResultFilter : IAsyncResultFilter
    {
        #region -    Fields    -

        private readonly ILogger<PostLocationHeaderResultFilter> _log;

        #endregion

        #region -    Constructors    -

        public PostLocationHeaderResultFilter(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<PostLocationHeaderResultFilter>();
        }

        #endregion

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            context.HttpContext.Response.OnStarting(() =>
                {
                    try
                    {
                        return HandleOnStartingAsync(context);
                    }
                    catch (Exception ex)
                    {
                        _log.LogCritical(() => $"Could not generate post location header.  Ex: {ex.Message}");
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
                    "ID property, therefore an GET url can not be created.  " +
                    "Hopefully this is your fault and not mine..."
                );

            return id.Value.ToString();
        }

        private Task HandleOnStartingAsync(ResultExecutingContext context)
        {
            var req = context.HttpContext.Request;
            var res = context.HttpContext.Response;

            if (!req.Method.EqualsEx("post")
                || !(context.Controller is ControllerBase controller)
                || res.StatusCode != StatusCodes.Status201Created &&
                res.StatusCode != StatusCodes.Status200OK)
            {
                _log.LogInformation(() =>
                    $"Skipping execute, Method: {req.Method}, " +
                    $"Controller: {context.Controller.GetType()}, StatusCode: {res.StatusCode}."
                );

                return Task.CompletedTask;
            }

            var cti = controller.ControllerContext.ActionDescriptor.ControllerTypeInfo;

            if (!cti.IsGenericType || !(context.Result is ObjectResult result))
                return Task.CompletedTask;

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

            return Task.CompletedTask;
        }
    }
}
