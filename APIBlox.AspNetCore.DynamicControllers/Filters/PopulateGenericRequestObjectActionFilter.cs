using System;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
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

        private readonly JsonSerializerSettings _settings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PopulateGenericRequestObjectActionFilter" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="resolver">The contract resolver</param>
        public PopulateGenericRequestObjectActionFilter(ILoggerFactory loggerFactory, IJsonBitsContractResolver resolver)
        {
            _log = loggerFactory.CreateLogger<PopulateGenericRequestObjectActionFilter>();

            _settings = new JsonSerializerSettings
            {
                ContractResolver = resolver
            };
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

        private void HandleExecute(ActionExecutingContext context)
        {
            var bits = new Bits(context, _settings);

            var requestObj = context.ActionArguments
                .FirstOrDefault(a => a.Value.GetType() == bits.RequestObjectType);

            if (!(requestObj.Value is null))
            {
                JsonConvert.PopulateObject(bits.RouteDataString, requestObj.Value, _settings);
                JsonConvert.PopulateObject(bits.QueryString, requestObj.Value, _settings);
            }
            else
            {
                var newModel = JsonConvert.DeserializeObject(bits.RouteDataString, bits.RequestObjectType, _settings);
                JsonConvert.PopulateObject(bits.QueryString, newModel, _settings);
            }
        }

        private class Bits
        {
            public Bits(ActionExecutingContext context, JsonSerializerSettings settings)
            {
                var q = context.HttpContext.Request.Query;
                var data = context.RouteData;
                var query = q.Keys.ToDictionary(k => k, v => q[v].FirstOrDefault());
                RequestObjectType = context.Controller.GetType().GetGenericArguments().First();
                var values = new RouteValueDictionary(data.Values.Where(kvp => !kvp.Key.EqualsEx("action") && !kvp.Key.EqualsEx("controller")));
                RouteDataString = JsonConvert.SerializeObject(values, settings);

                QueryString = JsonConvert.SerializeObject(query, settings);
            }
            
            public string QueryString { get; }

            public Type RequestObjectType { get; }

            public string RouteDataString { get; }
        }
    }
}
