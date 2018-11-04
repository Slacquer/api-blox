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

            // This will only be able to be displayed, it will not function as the
            // controller requires a query handler (CQRS).  Take a look at the CQRS example.
            configs.AddController<DynamicControllerRequest, DynamicControllerResponse, int>(
                route,
                "NoControllers",
                typeof(DynamicQueryByController<,,>)
            );
        }
    }
}
