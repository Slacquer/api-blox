using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    public class ChildPostRequest
    {
        
        /// <summary>
        ///     Gets or sets the parent identifier.
        /// </summary>
        /// <value>The parent identifier.</value>
        [FromRoute(Name = "parentId")]
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        [FromBody]
        public PersonModel Body { get; set; }
    }
}
