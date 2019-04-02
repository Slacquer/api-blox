using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <inheritdoc />
    /// <summary>
    ///     ByIdRequest.
    /// </summary>
    public class ByIdRequest : AllRequest
    {
        /// <summary>
        /// Gets or sets some identifier.
        /// </summary>
        /// <value>Some identifier.</value>
        [FromRoute(Name = "someId")]
        public int Id { get; set; }
    }
}
