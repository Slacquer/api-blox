using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.RequestsResponses;
using APIBlox.NetCore.Attributes;
using DemoApi2.Application.Locations.Queries;

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
