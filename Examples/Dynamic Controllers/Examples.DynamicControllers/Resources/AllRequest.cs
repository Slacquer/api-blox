using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <summary>
    /// Class AllRequest.
    /// </summary>
    public class AllRequest
    {
        /// <summary>
        /// The required value, and it must be three characters. please check out http://www.foo.com for more info.
        /// </summary>
        /// <value>The required value must be three characters.</value>
        [FromQuery(Name = "requiredValueMustBeThreeCharacters")]
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Value must be exactly 3 characters long.")]
        public string RequiredValueMustBeThreeCharacters { get; set; }
    }
}
