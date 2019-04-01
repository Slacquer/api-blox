﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <summary>
    ///     ByIdRequest.
    /// </summary>
    public class ByIdRequest
    {
        /// <summary>
        /// The required value, and it must be three characters. please check out http://www.foo.com for more info.
        /// </summary>
        /// <value>The required value must be three characters.</value>
        [FromQuery(Name = "requiredValueMustBeThreeCharacters")]
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Value must be exactly 3 characters long.")]
        public string RequiredValueMustBeThreeCharacters { get; set; }

        /// <summary>
        /// Gets or sets some identifier.
        /// </summary>
        /// <value>Some identifier.</value>
        [FromRoute(Name = "someId")]
        public int SomeId { get; set; }
    }
}