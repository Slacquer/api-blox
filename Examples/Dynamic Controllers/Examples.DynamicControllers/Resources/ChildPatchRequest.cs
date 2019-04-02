using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <summary>
    /// Class ChildPutRequest.
    /// </summary>
    public class ChildPatchRequest : ChildByIdRequest
    {
        /// <summary>
        /// Gets or sets an query value we want.
        /// </summary>
        /// <value>An query value we want.</value>
        [FromQuery]
        [Required]
        public int AQueryValueWeWant { get; set; }

        /// <summary>
        ///     Sets the body (patch document).
        /// </summary>
        /// <value>The body.</value>
        [FromBody]
        public JsonPatchDocument<PersonModel> Body { get; set; }
    }
}
