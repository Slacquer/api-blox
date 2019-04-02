using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <summary>
    ///     Class ParentPostRequest.
    /// </summary>
    public class ParentPostRequest
    {
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        [FromBody]
        public PersonModel Body { get; set; }
    }
}
