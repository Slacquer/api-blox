using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    internal class DynamicControllerRequest
    {
        [FromQuery(Name = "requiredValueMustBeThreeCharacters")]
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Value must be exactly 3 characters long.")]
        public string RequiredValueMustBeThreeCharacters { get; set; }

        [FromRoute(Name = "someId")]
        public int SomeId { get; set; }

        [FromRoute(Name = "id")]
        public int Id { get; set; }
    }
}
