using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class RouteTokensConvention : IApplicationModelConvention
    {
        private readonly Dictionary<string, string> _kvps;

        public RouteTokensConvention(Dictionary<string, string> kvps)
        {
            _kvps = kvps;
        }

        public void Apply(ApplicationModel application)
        {
            if (_kvps?.Any() != true)
                return;

            var camelCased = _kvps.Select(kvp =>
                new KeyValuePair<string, string>(kvp.Key.ToCamelCase(), kvp.Value.ToCamelCase())
            ).ToList();

            // Build a list of ALL selectors, and fix up tokens.
            var anonObjects = application.Controllers.Select(cm => new
                {
                    cm.ControllerName,
                    selectors = cm.Selectors.Where(sm => sm.AttributeRouteModel is not null)
                        .Concat(cm.Actions.SelectMany(am =>
                                am.Selectors.Where(sm => sm.AttributeRouteModel is not null)
                            )
                        )
                }
            );

            foreach (var anon in anonObjects)
            {
                var tmpAr = new Dictionary<string, string>(camelCased)
                {
                    {"controller", anon.ControllerName}
                };

                try
                {
                    foreach (var asm in anon.selectors)
                        asm.AttributeRouteModel.Template = AttributeRouteModel.ReplaceTokens(asm.AttributeRouteModel.Template, tmpAr);
                }
                catch (InvalidOperationException ioe)
                {
                    throw new InvalidOperationException(
                        "Route tokens are case sensitive, values found in configuration " +
                        "will be camel cased, you will need to alter what is found in your routes.",
                        ioe
                    );
                }
            }
        }
    }
}
