using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.RequestsResponses;
using APIBlox.NetCore.Attributes;
using DemoApi2.Domain.Contracts;
using DemoApi2.Domain.People;

// ReSharper disable once CheckNamespace
namespace DemoApi2.Application.People.Queries
{
    [InjectableService]
    public class GetAllPeopleQueryHandler : IQueryHandler<PagedPersonQuery, HandlerResponse>
    {
        private readonly IDomainDataService<PersonDomainModel, int> _dataService;

        public GetAllPeopleQueryHandler(IDomainDataService<PersonDomainModel, int> dataService)
        {
            _dataService = dataService;
        }

        public async Task<HandlerResponse> HandleAsync(PagedPersonQuery query, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var ret = new HandlerResponse();

            //var tmp =  await Task.FromResult(
            //    _dataService.GeTRequestQueryable().Select(
            //        m => new PersonResponse
            //        {
            //            BirthDate = m.BirthDate,
            //            FirstName = m.FirstName,
            //            Id = m.Id,
            //            LastName = m.LastName
            //        }
            //    )
            //).ConfigureAwait(false);


            var lst = new List<PersonResponse>();

            for (int i = 0; i < 1000; i++)
            {
                lst.Add(new PersonResponse {FirstName = "Foo" + i, Id = i,});
            }


            ret.Result = lst;

            return ret;
        }
    }
}
