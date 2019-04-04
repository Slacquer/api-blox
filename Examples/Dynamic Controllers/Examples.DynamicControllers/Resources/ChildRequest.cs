using APIBlox.AspNetCore.Types;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <summary>
    ///     Class ChildRequest.
    /// </summary>
    /// <seealso cref="APIBlox.AspNetCore.Types.FilteredQuery" />
    /// <inheritdoc />
    public class ChildrenRequest : FilteredQuery
    {
        /// <summary>
        ///     Gets or sets the parent identifier.
        /// </summary>
        /// <value>The parent identifier.</value>
        [FromRoute]
        public int ParentId { get; set; }
    }
}
