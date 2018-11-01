using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Controllers;
using Examples.Resources;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    internal static class FullDynamicConfiguration
    {
        public static void AddFullyDynamicConfiguration(this IDynamicControllerConfigurations configs)
        {
            // We can use our tokens here as well.
            var route = new[]
            {
                "api/noControllers/{someId}/dynamicControllerResources/{id}"
            };

            configs.AddController<DynamicControllerRequest, DynamicControllerResponse, int>(
                route,
                "noControllers",
                typeof(DynamicQueryByController<,,>)
            );
        }
    }
}
