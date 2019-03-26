using APIBlox.AspNetCore.Types;
using Examples.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <inheritdoc />
    internal class ChildRequest : FilteredQuery
    {
        [FromRoute(Name = "parentId")]
        public int ParentId { get; set; }

        [FromRoute(Name = "someRouteValueWeNeed")]
        public int SomeRouteValueWeNeed { get; set; }
    }
}
