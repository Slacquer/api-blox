#region -    Using Statements    -

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

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class DynamicControllerActionParametersConvention : IApplicationModelConvention
    {
        #region -    Fields    -

        private readonly IInternalDynamicControllerConfigurationsService _controllerConfigService;
        private readonly ILogger<DynamicControllerActionParametersConvention> _log;

        #endregion

        #region -    Constructors    -

        public DynamicControllerActionParametersConvention(
            ILoggerFactory loggerFactory,
            IInternalDynamicControllerConfigurationsService controllerConfigService
        )
        {
            _log = loggerFactory.CreateLogger<DynamicControllerActionParametersConvention>();
            _controllerConfigService = controllerConfigService;
        }

        #endregion

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

                    _log.LogInformation(() =>
                        $"Action final parameter(s) {string.Join(",", action.Parameters.Select(p => p.ParameterName))} " +
                        $"for action {action.Controller.ControllerName}.{action.ActionName}"
                    );
                }
            }
        }

        private bool IsGetOrDelete(ActionModel action)
        {
            if (!action.Attributes.Any())
                throw new ArgumentException(
                    $"Action {action.Controller.ControllerName}.{action.ActionName} " +
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
