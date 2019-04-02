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
    public class ChildPatchRequest
    {
        ///// <summary>
        ///// Sets the child id.
        ///// </summary>
        ///// <value>The identifier.</value>
        //[FromRoute(Name ="childId")]
        //public int Id { get; set; }

        ///// <summary>
        ///// Gets or sets the parent identifier.
        ///// </summary>
        ///// <value>The parent identifier.</value>
        //[FromRoute(Name = "parentId")]
        //public int ParentId { get; set; }

        ///// <summary>
        ///// Gets or sets an query value we want.
        ///// </summary>
        ///// <value>An query value we want.</value>
        //[FromQuery]
        //[Required]
        //public int AnQueryValueWeWant { get; set; }


        /// <summary>
        ///     Sets the body (patch document).
        /// </summary>
        /// <value>The body.</value>
        [FromBody]
        public JsonPatchDocument<PersonModel> Body { get; set; }
    }
}
