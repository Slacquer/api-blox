using APIBlox.AspNetCore.Types;

namespace Examples.Resources
{
    internal class ChildRequest : PaginationQuery
    {
        public int ParentId { get; private set; }

        // This is private so that things like swashbuckle won't try to
        // allow a user to fill it in, as we want it from the route.
        public int SomeRouteValueWeNeed { get; private set; }
    }
}
