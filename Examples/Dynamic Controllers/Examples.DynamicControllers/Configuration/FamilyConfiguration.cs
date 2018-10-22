using APIBlox.AspNetCore.Contracts;
using Examples.Controllers;
using Examples.Resources;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    internal static class FamilyConfiguration
    {
        public static void AddFamilyDynamicControllersConfiguration(this IDynamicControllerConfigurations configs)
        {
            // We can use our tokens here as well.
            var parentsRoutes = new[]
            {
                "api/dev-v[version]/resources/{SomeRouteValueWeNeed}/parents",
                "api/qa-v[version]/resources/parents",
                "api/prod-v[version]/resources/parents"
            };

            // Since kids is a sub route, we use a relative url(s), and they are built for us, so we only need one url.
            // With that being said, you COULD call AddController instead of AddSubController, and it NOT be considered a sub route.
            var kidsSubRoutes = new[] {"{parentId:int}/kids"};

            var parentController = configs.AddController<ParentRequest, ParentResponse, int>(
                parentsRoutes,
                typeof(FamilyDynamicGetController<,,>)
            );

            parentController.AddSubController<ChildRequest, ChildResponse, double>(
                typeof(int),
                kidsSubRoutes,
                typeof(FamilyDynamicGetController<,,>)
            );
        }
    }
}
