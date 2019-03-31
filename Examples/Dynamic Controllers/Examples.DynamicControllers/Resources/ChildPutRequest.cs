using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <summary>
    /// Class ChildPutRequest.
    /// </summary>
    public class ChildPutRequest
    {
        /// <summary>
        /// Sets the child id.
        /// </summary>
        /// <value>The identifier.</value>
        [FromRoute(Name ="childId")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>The parent identifier.</value>
        [FromRoute(Name = "parentId")]
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets some route value we need.
        /// </summary>
        /// <value>Some route value we need.</value>
        [FromRoute(Name = "someRouteValueWeNeed")]
        public int SomeRouteValueWeNeed { get; set; }


        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        [FromBody]
        public PersonModel Body { get; set; }
    }

    /// <summary>
    /// Class PersonModel.
    /// </summary>
    public class PersonModel
    {
        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>The age.</value>
        [Required]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [Required]
        public string FirstName { get; set; }
    }
}
