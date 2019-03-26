using System;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class PopulateGenericRequestObjectActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<PopulateGenericRequestObjectActionFilter> _log;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PopulateGenericRequestObjectActionFilter" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public PopulateGenericRequestObjectActionFilter(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<PopulateGenericRequestObjectActionFilter>();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.Controller.GetType();

            if (!controller.IsGenericType)
            {
                var cn = controller.Name;
                _log.LogInformation(() => $"Skipping execute for {cn} it isn't generic.");

                await next().ConfigureAwait(false);
            }

            HandleExecute(context);

            await next().ConfigureAwait(false);
        }

        private static void HandleExecute(ActionExecutingContext context)
        {
            var bits = new Bits(context);

            var (_, value) = context.ActionArguments
                .FirstOrDefault(a => a.Value.GetType() == bits.RequestObjectType);

            if (!(value is null))
            {
                JsonConvert.PopulateObject(bits.RouteDataString, value);
                JsonConvert.PopulateObject(bits.QueryString, value);
            }
            else
            {
                var newModel = JsonConvert.DeserializeObject(bits.RouteDataString, bits.RequestObjectType);
                JsonConvert.PopulateObject(bits.QueryString, newModel);
            }
        }

        private class Bits
        {
            public Bits(ActionExecutingContext context)
            {
                var q = context.HttpContext.Request.Query;
                var data = context.RouteData;
                var query = q.Keys.ToDictionary(k => k, v => q[v].FirstOrDefault());
                RequestObjectType = context.Controller.GetType().GetGenericArguments().First();
                var values = new RouteValueDictionary(data.Values.Where(kvp => !kvp.Key.EqualsEx("action") && !kvp.Key.EqualsEx("controller")));
                RouteDataString = JsonConvert.SerializeObject(values);

                QueryString = JsonConvert.SerializeObject(query);
            }

            public string QueryString { get; }

            public Type RequestObjectType { get; }

            public string RouteDataString { get; }
        }
    }
}
