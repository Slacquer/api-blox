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
                // SIDE NOTE: route values are case sensitive in swashbuckle UI.
                "api/dev-v[version]/resources/{someRouteValueWeNeed:int}/parents",
                //
                //  Uncomment to see how you could take advantage of multiple routes.
                //"api/qa-v[version]/resources/parents",
                //"api/prod-v[version]/resources/parents"
            };

            // Since children is a sub route, we use a relative url(s), and they are built for us, so we only need one url.
            // With that being said, you COULD call AddController instead of AddSubController, and it NOT be considered a sub route.
            var childrenSubRoutes = new[] {"{parentId:int}/children"};

            //
            //  Now technically we COULD use the same models over and over which would
            //  allow us to only call addController once for each resource, but that
            //  would be lazy, so stop thinking like that!
            //
            var parentController = configs.AddController<ParentRequest, ParentResponse, int>(
                parentsRoutes,
                typeof(FamilyDynamicGetController<,,>)
            );

            var parentPostController = configs.AddController<ParentPostRequest, ParentResponse, int>(
                parentsRoutes,
                typeof(FamilyDynamicPostController<,,>)
            );

            parentController.AddSubController<ChildRequest, ChildResponse, int>(
                typeof(int),
                childrenSubRoutes,
                typeof(FamilyDynamicGetController<,,>)
            );

            parentPostController.AddSubController<ChildPostRequest, ChildResponse, int>(
                typeof(int),
                childrenSubRoutes,
                typeof(FamilyDynamicPostController<,,>)
            );

            // We could create a ROOT level route for children and return result based on if they like candy or not
            // (Clearly I am having a tough time coming up with example data, don't judge me, sorry for partying...).
            var childrenRoutes = new[]  { "api/dev-v[version]/resources/children" };
            configs.AddController<ChildRootRequest, ChildResponse, int>(
                childrenRoutes,
                typeof(ChildrenDynamicGetAllController<,,>)
            );
        }
    }
}
