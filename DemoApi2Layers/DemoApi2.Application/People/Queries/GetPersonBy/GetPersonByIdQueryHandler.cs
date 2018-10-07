#region -    Using Statements    -

using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Attributes;
using DemoApi2.Domain.Contracts;
using DemoApi2.Domain.People;
using Newtonsoft.Json;

#endregion

// ReSharper disable once CheckNamespace
namespace DemoApi2.Application.People.Queries
{
    [InjectableService]
    public class GetPersonByIdQueryHandler : IQueryHandler<PersonQueryById, PersonResponse>
    {
        #region -    Fields    -

        private readonly IDomainDataService<PersonDomainModel, int> _dataService;

        #endregion

        #region -    Constructors    -

        public GetPersonByIdQueryHandler(IDomainDataService<PersonDomainModel, int> dataService)
        {
            _dataService = dataService;
        }

        #endregion

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
