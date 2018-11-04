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
                // SIDE NOTE: route values are case sensitive in swashbuckle UI, the need to match whatever the property name is.
                "api/dev-v0.0.1/resources/{SomeRouteValueWeNeed:int}/parents",
                //
                //  Comment out to make it more readable in the browser.
                "api/qa-v1-alpha/resources/parents",
                "api/prod-v1/resources/parents"
            };

            // Since children is a sub route, we use a relative url(s), and they are
            // built for us, so we only need one url.  With that being said, you COULD call
            // AddController instead of AddSubController, and it NOT be considered a sub route.
            var childrenSubRoutes = new[] {"{ParentId:int}/children"};

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
            var childrenRoutes = new[] { "api/dev-v0.0.1/resources/children/{LikesCandy:bool}" };
            configs.AddController<ChildRootRequest, ChildResponse, int>(
                childrenRoutes,
                //
                // We are specifying the controller name (which could have been children
                // like the other existing routes), as the logic behind the scenes uses the last
                // segment in the route to figure out the name, comment out the name and you will understand.
                "KidsWithHighDentalBills",
                typeof(ChildrenDynamicGetAllController<,,>)
            );
        }
    }
}
