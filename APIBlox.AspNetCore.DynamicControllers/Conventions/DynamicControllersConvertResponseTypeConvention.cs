using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class DynamicControllersConvertResponseTypeConvention : IApplicationModelConvention
    {
        private readonly ILogger<DynamicControllersConvertResponseTypeConvention> _log;

        public DynamicControllersConvertResponseTypeConvention(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<DynamicControllersConvertResponseTypeConvention>();
        }

        public void Apply(ApplicationModel application)
        {
            var actions = application.Controllers
                .Where(cm => cm.ControllerType.IsGenericType).SelectMany(cm => cm.Actions)
                .ToList();

            foreach (var actionModel in actions)
                Apply(actionModel);
        }

        private void Apply(ActionModel action)
        {
            var cType = action.Controller.ControllerType;

            if (!action.Attributes.Any())
                throw new ArgumentException(
                    $"Action {action.Controller.ControllerName}.{action.ActionName} " +
                    $"isn't decorated with an {nameof(HttpMethodAttribute)}."
                );

            var responseTypes = action.Attributes.OfType<ProducesResponseTypeAttribute>()
                .Where(rt => rt.Type != default(Type))
                .ToList();

            if (!responseTypes.Any())
            {
                _log.LogWarning(() =>
                    $"Action {action.Controller.ControllerName}.{action.ActionName} " +
                    $"doesn't have any {nameof(ProducesResponseTypeAttribute)}s"
                );

                return;
            }

            var t = cType.GenericTypeArguments[cType.GenericTypeArguments.Length == 1 ? 0 : 1];

            foreach (var rt in responseTypes)
            {
                if (typeof(IEnumerable).IsAssignableTo(rt.Type))
                    rt.Type = typeof(IEnumerable<>).MakeGenericType(t);

                else if (typeof(IResource).IsAssignableTo(rt.Type)) // Using marker interface
                    rt.Type = t;
            }
        }
    }
}
