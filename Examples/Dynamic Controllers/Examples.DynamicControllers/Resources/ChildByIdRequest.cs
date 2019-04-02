using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <summary>
    ///     Class ChildByIdRequest.
    /// </summary>
    public class ChildByIdRequest
    {
        /// <summary>
        ///     Sets the child id.
        /// </summary>
        [FromRoute(Name = "childId")]
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
