#region -    Using Statements    -

using System;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.CommandQueryResponses;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Extensions;
using APIBlox.NetCore.Attributes;
using DemoApi2.Application.Locations;
using DemoApi2.Application.Locations.Queries;
using Newtonsoft.Json;

#endregion

// ReSharper disable once CheckNamespace
namespace DemoApi2.Application.People.Queries
{
    [InjectableService]
    public class GetLocationByIdHandler : IQueryHandler<LocationQuery, HandlerResponse>
    {
        public Task<HandlerResponse> HandleAsync(LocationQuery query, CancellationToken cancellationToken)
        {
            var ret = new HandlerResponse();

            
            

            return Task.FromResult(ret);
        }
    }
}
