using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APIBlox.AspNetCore.Contracts
{
    internal interface IPaginationBuilder
    {
        dynamic Build(IEnumerable<object> result, ActionExecutingContext context);
    }
}
