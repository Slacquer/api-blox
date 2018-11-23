using APIBlox.AspNetCore.Types;
using Examples.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    internal class ChildRootRequest : PaginationQuery, IChildRequest
    {
        [FromQuery(Name = "likesCandy")] 
        public bool LikesCandy { get; set; }
    }
}
