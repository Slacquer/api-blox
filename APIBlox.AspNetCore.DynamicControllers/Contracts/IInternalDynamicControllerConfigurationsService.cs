#region -    Using Statements    -

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

#endregion

namespace APIBlox.AspNetCore.Contracts
{
    internal interface IInternalDynamicControllerConfigurationsService
    {
        List<DynamicControllerConfiguration> ControllerConfigurations { get; }

        List<ParameterModel> Parameters { get; set; }

        void AddControllerConfig(DynamicControllerConfiguration controllerConfig);
    }
}
