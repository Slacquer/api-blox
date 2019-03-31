using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    public class ChildByIdRequest
    {
        /// <summary>
        /// Sets the child id.
        /// </summary>
        [FromRoute]
        public int Id { get; set; }

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
