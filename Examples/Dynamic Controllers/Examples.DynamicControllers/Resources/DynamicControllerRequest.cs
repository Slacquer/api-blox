using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    public class DynamicControllerRequest
    {
        public int Id { get; private set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [FromQuery]
        public string RequiredValueMustBeThreeCharacters { get; set; }

        public int SomeId { get; private set; }
    }
}
