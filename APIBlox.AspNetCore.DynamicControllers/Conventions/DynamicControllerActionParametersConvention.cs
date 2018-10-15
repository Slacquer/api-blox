using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class DynamicControllerActionParametersConvention : IApplicationModelConvention
    {
        private readonly IInternalDynamicControllerConfigurationsService _controllerConfigService;
        private readonly ILogger<DynamicControllerActionParametersConvention> _log;

        public DynamicControllerActionParametersConvention(
            ILoggerFactory loggerFactory,
            IInternalDynamicControllerConfigurationsService controllerConfigService
        )
        {
            _log = loggerFactory.CreateLogger<DynamicControllerActionParametersConvention>();
            _controllerConfigService = controllerConfigService;
        }

        /// <inheritdoc />
        public void Apply(ApplicationModel application)
        {
            // For each generic controller we want to add action parameters for 2 things
            // 1) each of the constraints in each of its routes
            // 2) The TRequest properties.

            var actions = application.Controllers.Where(c =>
                    c.ControllerType.IsGenericType
                ).SelectMany(cm => cm.Actions)
                .ToList();

            foreach (var action in actions)
            {
                AddParameters(action);

                ReorderParameters(action);
            }
        }

        private void AddParameters(ActionModel action)
        {
            foreach (var sm in action.Controller.Selectors)
            {
                var lst = new List<ParameterModel>();

                if (sm.AttributeRouteModel?.Template?.Contains("{") == true)
                    lst.AddRange(AddRouteTemplateParameters(sm.AttributeRouteModel.Template));

                if (IsGetOrDelete(action))
                    foreach (var pm in AddRequestObjectParameters(action.Controller.ControllerType.GetGenericArguments()[0]))
                    {
                        if (!lst.Any(p => p.Name.EqualsEx(pm.Name)))
                            lst.Add(pm);
                    }

                if (!lst.Any())
                    continue;

                _log.LogInformation(() =>
                    $"Adding parameter(s) {string.Join(",", lst.Select(p => p.ParameterName))} " +
                    $"to action {action.Controller.ControllerName}.{action.ActionName}"
                );

                foreach (var pm in lst)
                {
                    if (!action.Parameters.Any(p => p.Name.EqualsEx(pm.Name)))
                        action.Parameters.Insert(0, pm);
                }
            }
        }

        private void ReorderParameters(ActionModel action)
        {
            var bodyParams = action.Parameters
                .Where(p => !(p.BindingInfo is null))// && p.BindingInfo.BindingSource.DisplayName.ContainsEx("body"))
                .OrderBy(p => p.ParameterName.EndsWithEx("id"))
                .ThenBy(p => p.ParameterName).ToList();

            var nonBody = action.Parameters
                .Except(bodyParams)
                .OrderBy(p => p.ParameterName.EndsWithEx("id"))
                .ThenBy(p => p.ParameterName).ToList();

            action.Parameters.Clear();

            foreach (var pm in nonBody)
                action.Parameters.Add(pm);

            foreach (var pm in bodyParams)
                action.Parameters.Add(pm);

            _log.LogInformation(() =>
                $"Action final ORDERED parameter(s) {string.Join(",", action.Parameters.Select(p => p.ParameterName))} " +
                $"for action {action.Controller.ControllerName}.{action.ActionName}"
            );
        }

        private bool IsGetOrDelete(ActionModel action)
        {
            if (!action.Attributes.Any())
                throw new ArgumentException($"Action {action.Controller.ControllerName}.{action.ActionName} " +
                                            $"isn't decorated with an {nameof(HttpMethodAttribute)}."
                );

            return action.Attributes.OfType<HttpMethodAttribute>()
                .Any(m => m is HttpGetAttribute || m is HttpDeleteAttribute);
        }

        private IEnumerable<ParameterModel> AddRouteTemplateParameters(string template)
        {
            var templateParams = TemplateParser.Parse(template).Parameters;
            var templateParts = templateParams.Where(tp =>
                tp.InlineConstraints.Any()
            ).SelectMany(t =>
                t.InlineConstraints.Select(tt =>
                    GetParameter(tt.Constraint, t.Name)
                )
            ).ToList();

            if (templateParams.Count != templateParts.Count)
                _log.LogWarning(() =>
                    "Route template has inline parameters that do NOT have " +
                    "constraints, if the non constrained parameter(s) are of type STRING " +
                    "than this message can be ignored, otherwise " +
                    $"unexpected results may occur.  Route Template: {template}"
                );

            return templateParts.Where(p => p != null);
        }

        private IEnumerable<ParameterModel> AddRequestObjectParameters(IReflect requestType)
        {
            var props = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.SetMethod.IsPublic
                            && (p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                );

            return props
                .Select(pi => GetParameter(pi.PropertyType.Name, pi.Name))
                .Where(p => p != null);
        }

        private ParameterModel GetParameter(string typeName, string parameterName)
        {
            var parameter = _controllerConfigService.Parameters.FirstOrDefault(p =>
                p.DisplayName.StartsWithEx(typeName) || p.ParameterType.Name.EqualsEx(typeName)
            );

            if (parameter is null)
                return null;

            return new ParameterModel(parameter)
            {
                ParameterName = parameterName.ToCamelCase()
            };
        }
    }
}
