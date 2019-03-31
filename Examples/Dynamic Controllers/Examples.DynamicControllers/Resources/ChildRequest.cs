﻿#region -    Using Statements    -

using APIBlox.AspNetCore.Types;
using Microsoft.AspNetCore.Mvc;

#endregion

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

        /// <summary>
        ///     Gets or sets some route value we need.
        /// </summary>
        /// <value>Some route value we need.</value>
        [FromRoute]
        public int SomeRouteValueWeNeed { get; set; }
    }
}
