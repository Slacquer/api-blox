#region -    Using Statements    -

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Attributes;
using DemoApi2.Domain.Contracts;
using DemoApi2.Domain.People;

#endregion

// ReSharper disable once CheckNamespace
namespace DemoApi2.Application.People.Queries
{
    [InjectableService]
    public class GetAllPeopleQueryHandler : IQueryHandler<PersonQuery, IEnumerable<PersonResponse>>
    {
        #region -    Fields    -

        private readonly IDomainDataService<PersonDomainModel, int> _dataService;

        #endregion

        #region -    Constructors    -

        public GetAllPeopleQueryHandler(IDomainDataService<PersonDomainModel, int> dataService)
        {
            _dataService = dataService;
        }

        #endregion

        /// <inheritdoc />
        public async Task<IEnumerable<PersonResponse>> HandleAsync(PersonQuery query, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(
                _dataService.GeTRequestQueryable().Select(
                    m => new PersonResponse
                    {
                        BirthDate = m.BirthDate,
                        FirstName = m.FirstName,
                        Id = m.Id,
                        LastName = m.LastName
                    }
                )
            ).ConfigureAwait(false);
        }
    }
}
