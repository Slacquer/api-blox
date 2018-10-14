using APIBlox.AspNetCore.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APIBlox.AspNetCore.Contracts
{
    internal interface IPaginationMetadataBuilder
    {
        PaginationMetadata Build(int resultCount, ActionExecutingContext context);
    }
}
