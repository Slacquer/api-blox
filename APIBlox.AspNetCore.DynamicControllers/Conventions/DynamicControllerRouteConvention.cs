using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class DynamicControllersRouteConvention : IApplicationModelConvention
    {
        private readonly IInternalDynamicControllerConfigurationsService _controllerConfigService;

        public DynamicControllersRouteConvention(IInternalDynamicControllerConfigurationsService controllerConfigService)
        {
            _controllerConfigService = controllerConfigService;
        }

        /// <inheritdoc />
        public void Apply(ApplicationModel application)
        {
            var controllers = application.Controllers
                .Where(cm => cm.ControllerType.IsGenericType).ToList();

            foreach (var controllerModel in controllers)
                Apply(controllerModel);
        }

        private static void ClearDefaultSelector(ControllerModel controller)
        {
            // Remove base selector.
            var selectors = controller.Selectors.Where(sm =>
                sm.AttributeRouteModel?.Template.Contains("[controller]") == true
            ).ToList();

            foreach (var sm in selectors)
                controller.Selectors.Remove(sm);
        }

        private static void SetControllerNameAndAddSelectors(
            ControllerModel controller,
            DynamicControllerConfiguration controllerConfiguration
        )
        {
            controller.ControllerName = controllerConfiguration.ControllerName.ToPascalCase();

            foreach (var r in controllerConfiguration.Routes)
            {
                controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel
                        {
                            Template = r
                        }
                    }
                );
            }
        }

        private void Apply(ControllerModel controller)
        {
            ClearDefaultSelector(controller);
            var controllerConfigs = GetControllerConfigurations(controller);

            if (!controllerConfigs.Any())
                return;

            foreach (var controllerConfiguration in controllerConfigs)
                SetControllerNameAndAddSelectors(controller, controllerConfiguration);
        }

        private List<DynamicControllerConfiguration> GetControllerConfigurations(ControllerModel controller)
        {
            var controllerConfigs = _controllerConfigService.ControllerConfigurations
                .Flatten(c => c.SubResourceControllers)
                .Where(b => b.ControllerType == controller.ControllerType)
                .ToList();


            return controllerConfigs;
        }
    }
}
