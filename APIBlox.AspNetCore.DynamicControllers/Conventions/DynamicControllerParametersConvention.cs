using System.Linq;
using APIBlox.AspNetCore.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class DynamicControllerParametersConvention : IApplicationModelConvention
    {
        private readonly IInternalDynamicControllerConfigurationsService _controllerConfigService;

        public DynamicControllerParametersConvention(IInternalDynamicControllerConfigurationsService controllerConfigService)
        {
            _controllerConfigService = controllerConfigService;
        }

        /// <inheritdoc />
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

            var t = cType.GenericTypeArguments[0];

            var genericParam = action.Parameters.FirstOrDefault(p =>
                p.ParameterType == t && p.Attributes.Any(a =>
                    a.GetType() == typeof(FromQueryAttribute)
                )
            );

            if (genericParam is null)
                return;

            // Since all of the MVC bits have already run and did their thing, we can safely
            // reset the [FromQuery] attribute of the param, so that any bits AFTER us
            // (IE: Swashbuckle) will not want to add them as query params.
            genericParam.BindingInfo.BindingSource = null;
        }
    }
}
