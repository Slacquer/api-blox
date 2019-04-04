using APIBlox.AspNetCore.Types;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <summary>
    ///     Class ParentRequest.
    /// </summary>
    public class ParentRequest : FilteredPaginationQuery
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [FromRoute(Name = "parentId")]
        public int Id { get; set; }
    }
}
