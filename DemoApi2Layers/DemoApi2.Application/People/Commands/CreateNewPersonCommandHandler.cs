#region -    Using Statements    -

using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.CommandQueryResponses;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Extensions;
using APIBlox.AspNetCore.RequestsResponses;
using APIBlox.NetCore.Attributes;
using DemoApi2.Domain.Contracts;
using DemoApi2.Domain.People;
using Newtonsoft.Json;

#endregion

namespace DemoApi2.Application.People.Commands
{
    [InjectableService]
    public class CreateNewPersonCommandHandler :
        ICommandHandler<PersonCommand, HandlerResponse>
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

        public async Task<HandlerResponse> HandleAsync(PersonCommand requestCommand, CancellationToken cancellationToken)
        {
            var ret = new HandlerResponse();
            
            // At this point Request validation has been done.
            // And if decorated with a Domain validation decorator it is done too.
            // Should be able to simply insert.
            var model = JsonConvert.DeserializeObject<PersonDomainModel>(JsonConvert.SerializeObject(requestCommand));
            
            if (model.EmailAddress == "0")
            {
                ret.NewError().SetErrorToForbidden();
                return ret;
            }
            if (model.EmailAddress == "1")
            {
                ret.NewError().SetErrorToDataConflict();
                return ret;
            }
            if (model.EmailAddress == "2")
            {
                ret.NewError().SetErrorToDataConflictUpserts();
                return ret;
            }
            if (model.EmailAddress == "4")
            {
                ret.NewError().SetErrorToNotFound();
                return ret;
            }
            if (model.EmailAddress == "5")
            {
                ret.NewError().SetErrorToUnAuthorized();
                return ret;
            }

            var retModel = _dataService.Create(model);
            await _dataService.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            ret.Result = JsonConvert.DeserializeObject<PersonResponse>(
                JsonConvert.SerializeObject(retModel)
            );

            return ret;
        }
    }
}
