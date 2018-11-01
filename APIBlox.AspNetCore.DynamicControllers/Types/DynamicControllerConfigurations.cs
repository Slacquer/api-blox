using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class DynamicControllerConfigurationsService : IDynamicControllerConfigurations,
        IInternalDynamicControllerConfigurationsService
    {
        public List<DynamicControllerConfiguration> ControllerConfigurations { get; } =
            new List<DynamicControllerConfiguration>();

        public List<ParameterModel> Parameters { get; set; } = new List<ParameterModel>();
        //public List<ParameterModel> RequiredParameters { get; set; } = new List<ParameterModel>();

        public void AddControllerConfig(DynamicControllerConfiguration controllerConfig)
        {
            var routes = controllerConfig.Routes.Distinct().ToList();
            var first = routes.First();
            var name = controllerConfig.ControllerName ?? first.Substring(first.LastIndexOfEx("/") + 1);
            var internalName = name;
            var flat = ControllerConfigurations.Flatten(c => c.SubResourceControllers).ToList();

            if (!CheckExistsAndAddRoutesIfNecessary(controllerConfig, flat))
                AddWithInternalName(controllerConfig, flat, name, internalName);

            ControllerConfigurations.Remove(controllerConfig);
        }

        private static bool CheckExistsAndAddRoutesIfNecessary(
            DynamicControllerConfiguration controllerConfig,
            IEnumerable<DynamicControllerConfiguration> flat
        )
        {
            var existing = flat.FirstOrDefault(c => c.ControllerType == controllerConfig.ControllerType);

            if (!existing)
                return false;

            existing.Routes.AddRange(controllerConfig.Routes.Except(existing.Routes));

            return true;
        }

        private void AddWithInternalName(
            DynamicControllerConfiguration controllerConfig,
            IEnumerable<DynamicControllerConfiguration> flat, string name, string internalName
        )
        {
            var names = flat.Select(c => c.ControllerName)
                .Where(s => s.EqualsEx(name)).ToList();

            var count = names.Any()
                ? names.Count()
                : 0;

            var config = new DynamicControllerConfiguration(name,
                controllerConfig.RequestResourceType,
                controllerConfig.ParentIdType,
                controllerConfig.Routes.ToArray()
            )
            {
                ControllerType = controllerConfig.ControllerType,
                ParentController = controllerConfig.ParentController,
                SubResourceControllers = controllerConfig.SubResourceControllers,
                InternalControllerName = $"{internalName}{count}"
            };

            ControllerConfigurations.Add(config);
        }
    }
}
