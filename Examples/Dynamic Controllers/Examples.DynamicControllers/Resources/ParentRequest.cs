using System.ComponentModel.DataAnnotations;
using APIBlox.AspNetCore.Types;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    internal class ParentRequest : PaginationQuery
    {
        [Required]
        [FromQuery]
        public string SomeOtherThingWeNeedToKnowWhenRequestingData { get; set; }

        [FromRoute(Name = "someRouteValueWeNeed")]
        public int SomeRouteValueWeNeed { get; set; }
    }
}
