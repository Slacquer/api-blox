#region -    Using Statements    -

using System;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class PopulateGenericRequestObjectActionFilter : IAsyncActionFilter
    {
        #region -    Fields    -

        private readonly ILogger<PopulateGenericRequestObjectActionFilter> _log;

        private readonly JsonSerializerSettings _settings;

        #endregion

        #region -    Constructors    -

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

        #endregion

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

            if (bits.IsQuery)
            {
                var req = JsonConvert.DeserializeObject(bits.RouteDataString, bits.RequestObjectType, _settings);
                JsonConvert.PopulateObject(bits.QueryString, req, _settings);
                SetViewData(context, bits.RequestObjectType.Name, req);
            }
            else
            {
                var existingModel = context.ActionArguments
                    .FirstOrDefault(a => a.Value.GetType() == bits.RequestObjectType);

                if (!(existingModel.Value is null))
                {
                    JsonConvert.PopulateObject(bits.RouteDataString, existingModel.Value, _settings);
                    JsonConvert.PopulateObject(bits.QueryString, existingModel.Value, _settings);
                }
                else
                {
                    var newModel = JsonConvert.DeserializeObject(bits.RouteDataString, bits.RequestObjectType, _settings);
                    JsonConvert.PopulateObject(bits.QueryString, newModel, _settings);
                    SetViewData(context, bits.RequestObjectType.Name, newModel);
                }
            }
        }

        private void SetViewData(ActionExecutingContext context, string name, object req)
        {
            var cn = context.Controller.GetType().Name;
            var an = context.ActionDescriptor.DisplayName;

            if (!(context.Controller is Controller c))
            {
                context.RouteData.Values.Add(name, req);
                _log.LogInformation(() => $"Set RouteData.Value: Key={name} for {cn}/{an}");
            }
            else
            {
                c.ViewData[name] = req;
                _log.LogInformation(() => $"Set ViewData.Value: Key={name} for {cn}/{an}");
            }
        }

        #region -    Nested type: Bits    -

        private class Bits
        {
            #region -    Constructors    -

            public Bits(ActionExecutingContext context, JsonSerializerSettings settings)
            {
                var q = context.HttpContext.Request.Query;
                var data = context.RouteData;
                var query = q.Keys.ToDictionary(k => k, v => q[v].FirstOrDefault());
                RequestObjectType = context.Controller.GetType().GetGenericArguments().First();
                IsQuery = context.HttpContext.Request.Method.EqualsEx("get");
                var values = new RouteValueDictionary(data.Values.Where(kvp => !kvp.Key.EqualsEx("action") && !kvp.Key.EqualsEx("controller")));
                RouteDataString = JsonConvert.SerializeObject(values, settings);

                QueryString = JsonConvert.SerializeObject(query, settings);
            }

            #endregion

            public bool IsQuery { get; }

            public string QueryString { get; }

            public Type RequestObjectType { get; }

            public string RouteDataString { get; }
        }

        #endregion
    }
}
