﻿using System.ComponentModel.DataAnnotations;
using APIBlox.AspNetCore.Types;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <summary>
    ///     Class ParentRequest.
    /// </summary>
    public class ParentRequest : FilteredPaginationQuery
    {
        /// <summary>
        ///     Gets or sets some other thing we need to know when requesting data.
        /// </summary>
        /// <value>Some other thing we need to know when requesting data.</value>
        [Required]
        [FromQuery]
        public string SomeOtherThingWeNeedToKnowWhenRequestingData { get; set; }

        /// <summary>
        ///     Gets or sets some route value we need.
        /// </summary>
        /// <value>Some route value we need.</value>
        [FromRoute]
        public int SomeRouteValueWeNeed { get; set; }
    }
}
