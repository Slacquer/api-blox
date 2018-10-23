using System.ComponentModel.DataAnnotations;
using APIBlox.AspNetCore.Types;
using Examples.Contracts;

namespace Examples.Resources
{
    public class ChildRootRequest : PaginationQuery, IChildRequest
    {
        public bool LikesCandy { get;set; }
    }
}
