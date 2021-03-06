﻿using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <summary>
    ///     Class ChildPutRequest.
    /// </summary>
    public class ChildPutRequest : ChildByIdRequest
    {
        /// <summary>
        ///     Gets or sets an query value we want.
        /// </summary>
        /// <value>An query value we want.</value>
        [FromQuery]
        public int? AnQueryValueWeWant { get; set; }

        /// <summary>
        ///     Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        [FromBody]
        public PersonModel Body { get; set; }
    }
}
