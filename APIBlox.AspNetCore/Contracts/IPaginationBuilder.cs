using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APIBlox.AspNetCore.Contracts
{
    internal interface IPaginationBuilder
    {
        PaginationResultBits Build(int resultCount, ActionExecutingContext context);
    }
}
