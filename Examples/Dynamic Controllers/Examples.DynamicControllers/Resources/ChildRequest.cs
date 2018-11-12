using APIBlox.AspNetCore.Types;
using Examples.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <inheritdoc />
    internal class ChildRequest : FilteredQuery
    {
        [FromRoute]
        public int ParentId { get; private set; }
    }
}
