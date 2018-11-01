using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Examples.Resources
{
    public class DynamicControllerRequest
    {
        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string RequiredValueMustBeThreeCharacters { get; set; }


        public int SomeId { get; private set; }

        public int Id { get; private set; }

    }
}
