using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace APIBlox.AspNetCore.Contracts
{
    internal interface IInternalDynamicControllerConfigurationsService
    {
        List<DynamicControllerConfiguration> ControllerConfigurations { get; }

        List<ParameterModel> Parameters { get; set; }
        List<ParameterModel> RequiredParameters { get; set; }

        void AddControllerConfig(DynamicControllerConfiguration controllerConfig);
    }
}
