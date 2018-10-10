using System;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Attributes;

namespace DemoApi2.Application.People.Commands
{
    [InjectableService]
    public class DeletePersonByKeyCommandHandler : ICommandHandler<DeletePersonByKeyCommand, PersonResponse>
    {
        public Task<PersonResponse> HandleAsync(DeletePersonByKeyCommand requestCommand, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
