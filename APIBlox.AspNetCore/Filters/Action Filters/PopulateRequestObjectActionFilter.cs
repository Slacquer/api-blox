﻿#region -    Using Statements    -

using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Attributes;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.JsonBits;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class PopulateRequestObjectActionFilter : IAsyncActionFilter
    {
        #region -    Fields    -

        private readonly ILogger<PopulateRequestObjectActionFilter> _log;

        private readonly JsonSerializerSettings _settings;

        #endregion

        #region -    Constructors    -

        public PopulateRequestObjectActionFilter(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<PopulateRequestObjectActionFilter>();

            _settings = new JsonSerializerSettings
            {
                ContractResolver = new PopulateNonPublicSettersContractResolver()
            };
        }

        #endregion

        /// <inheritdoc />
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var bits = new Bits(context);

            if (bits.RequestModelObject is null)
            {
                await next().ConfigureAwait(false);
                return;
            }

            Handle(context, bits);

            await next().ConfigureAwait(false);
        }

        private void Handle(ActionExecutingContext context, Bits bits)
        {
            if (!bits.IsQuery)
            {
                JsonConvert.PopulateObject(bits.RouteDataString, bits.RequestModelObject, _settings);
                JsonConvert.PopulateObject(bits.QueryString, bits.RequestModelObject, _settings);
                SetViewData(context, bits.RequestModelObject.GetType().Name, bits.RequestModelObject);
            }
            else
            {
                var req = JsonConvert.DeserializeObject(bits.RouteDataString,
                    bits.RequestModelObject.GetType(),
                    _settings
                );
                JsonConvert.PopulateObject(bits.QueryString, req, _settings);
                SetViewData(context, bits.RequestModelObject.GetType().Name, req);
            }

            _log.LogInformation(() =>
                {
                    var cn = context.Controller.GetType().Name;
                    var an = context.ActionDescriptor.DisplayName;

                    return $"Set RouteData.Value: Key={bits.RequestModelObject.GetType().Name} for {cn}/{an}";
                }
            );
        }

        private static void SetViewData(ActionExecutingContext context, string name, object req)
        {
            if (!(context.Controller is Controller c))
                context.RouteData.Values.Add(name, req);
            else
                c.ViewData[name] = req;
        }

        #region -    Nested type: Bits    -

        private class Bits
        {
            #region -    Constructors    -

            public Bits(ActionExecutingContext context)
            {
                var first = context.ActionDescriptor.Parameters.FirstOrDefault(p =>
                    !(((ControllerParameterDescriptor) p).ParameterInfo.GetCustomAttribute<PopulateAttribute>() is null)
                );

                if (first is null)
                    return;

                RequestModelObject = context.ActionArguments.Select(kvp => kvp.Value)
                    .FirstOrDefault(o => o.GetType() == first.ParameterType);

                var q = context.HttpContext.Request.Query;
                var data = context.RouteData;
                var query = q.Keys.ToDictionary(k => k, v => q[v].FirstOrDefault());

                IsQuery = context.HttpContext.Request.Method.EqualsEx("get");
                RouteDataString = JsonConvert.SerializeObject(data.Values);
                QueryString = JsonConvert.SerializeObject(query);
            }

            #endregion

            public bool IsQuery { get; }

            public string QueryString { get; }

            public object RequestModelObject { get; }

            public string RouteDataString { get; }
        }

        #endregion
    }
}
