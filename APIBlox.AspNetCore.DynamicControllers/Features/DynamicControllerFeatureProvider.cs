using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class DynamicControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IInternalDynamicControllerConfigurationsService _service;

        private ControllerFeature _feature;

        public DynamicControllerFeatureProvider(IInternalDynamicControllerConfigurationsService service)
        {
            _service = service;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if (!_service.ControllerConfigurations.Any())
                throw new ArgumentException(
                    "No dynamic controller configurations were found.  " +
                    $"If this is intentional, then do not use {nameof(DynamicControllerFeatureProvider)}."
                );

            _feature = feature;

            var lst = _service.ControllerConfigurations.Flatten(c => c.SubResourceControllers).ToList();

            foreach (var config in lst)
                AddController(config);

            //feature.Controllers.Add(typeof(HowDoICreateAParameterModelWithoutDoingThisCrapController).GetTypeInfo());
        }

        private void AddController(DynamicControllerConfiguration config)
        {
            var ti = config.ControllerType.GetTypeInfo();

            if (!_feature.Controllers.Contains(ti))
                _feature.Controllers.Add(ti);
        }
    }
}
