using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Attributes;
using DemoApi2.Domain.Contracts;
using DemoApi2.Domain.People;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace DemoApi2.Application.People.Queries
{
    [InjectableService]
    public class GetPersonByIdQueryHandler : IQueryHandler<PersonQueryById, PersonResponse>
    {
        private readonly IDomainDataService<PersonDomainModel, int> _dataService;

        public GetPersonByIdQueryHandler(IDomainDataService<PersonDomainModel, int> dataService)
        {
            _dataService = dataService;
        }

        /// <inheritdoc />
        public async Task<PersonResponse> HandleAsync(PersonQueryById query, CancellationToken cancellationToken)
        {
            var result = await _dataService.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);

            return result is null
                ? null
                : JsonConvert.DeserializeObject<PersonResponse>(JsonConvert.SerializeObject(result));
        }
    }
}
