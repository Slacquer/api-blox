using System.Collections.Generic;
using System.Linq;
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

            // Build a list of ALL selectors, and fix up tokens.
            var anonObjects = application.Controllers.Select(cm => new
                {
                    cm.ControllerName,
                    selectors = cm.Selectors.Where(sm => !(sm.AttributeRouteModel is null))
                        .Concat(cm.Actions.SelectMany(am =>
                                am.Selectors.Where(sm => sm.AttributeRouteModel != null)
                            )
                        )
                }
            );

            foreach (var anon in anonObjects)
            {
                var tmpAr = new Dictionary<string, string>(_kvps)
                {
                    {"controller", anon.ControllerName}
                };

                foreach (var asm in anon.selectors)
                    asm.AttributeRouteModel.Template = AttributeRouteModel.ReplaceTokens(asm.AttributeRouteModel.Template, tmpAr);
            }
        }
    }
}
