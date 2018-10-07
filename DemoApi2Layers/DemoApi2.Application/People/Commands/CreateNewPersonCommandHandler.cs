#region -    Using Statements    -

using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Attributes;
using DemoApi2.Domain.Contracts;
using DemoApi2.Domain.People;
using Newtonsoft.Json;

#endregion

namespace DemoApi2.Application.People.Commands
{
    [InjectableService]
    public class CreateNewPersonCommandHandler :
        ICommandHandler<PersonCommand, PersonResponse>
    {
        #region -    Fields    -

        private readonly IDomainDataService<PersonDomainModel, int> _dataService;

        #endregion

        #region -    Constructors    -

        public CreateNewPersonCommandHandler(IDomainDataService<PersonDomainModel, int> dataService)
        {
            _dataService = dataService;
        }

        #endregion

        /// <inheritdoc />
        public async Task<PersonResponse> HandleAsync(
            PersonCommand requestCommand,
            CancellationToken cancellationToken
        )
        {
            // At this point Request validation has been done.
            // And if decorated with a Domain validation decorator it is done too.
            // Should be able to simply insert.
            var model = JsonConvert.DeserializeObject<PersonDomainModel>(JsonConvert.SerializeObject(requestCommand));
            var ret = _dataService.Create(model);
            await _dataService.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<PersonResponse>(JsonConvert.SerializeObject(ret));
        }
    }
}
