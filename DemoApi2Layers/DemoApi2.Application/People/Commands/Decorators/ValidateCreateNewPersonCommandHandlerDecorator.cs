using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using DemoApi2.Domain.Contracts;
using DemoApi2.Domain.People;
using Microsoft.Extensions.Logging;

namespace DemoApi2.Application.People.Commands.Decorators
{
    public class ValidateCreateNewPersonCommandHandlerDecorator :
        ICommandHandler<PersonCommand, PersonResponse>
    {
        private readonly IDomainDataService<PersonDomainModel, int> _dataService;
        private readonly ICommandHandler<PersonCommand, PersonResponse> _decorated;
        private readonly ILogger<ValidateCreateNewPersonCommandHandlerDecorator> _log;

        public ValidateCreateNewPersonCommandHandlerDecorator(
            ILoggerFactory loggerFactory,
            ICommandHandler<PersonCommand, PersonResponse> decorated,
            IDomainDataService<PersonDomainModel, int> dataService
        )
        {
            _log = loggerFactory.CreateLogger<ValidateCreateNewPersonCommandHandlerDecorator>();
            _decorated = decorated;
            _dataService = dataService;
        }

        public async Task<PersonResponse> HandleAsync(
            PersonCommand requestCommand,
            CancellationToken cancellationToken
        )
        {
            _log.LogInformation(() => $"Verifying person '{requestCommand.FirstName}' does not already exist.");

            //var exists = _dataService.GeTRequestQueryable()
            //    .FirstOrDefault(m =>
            //        m.FirstName.Equals(command.RequestResource.FirstName, StringComparison.InvariantCultureIgnoreCase)
            //    );

            //if (exists)
            //    throw new ConflictException(new ErrorResponseObject
            //    {
            //        Code = ErrorResponseCodes.DataConflict,
            //        Message = $"Person '{command.RequestResource.FirstName}' already exists.",
            //        Target = nameof(command.RequestResource.FirstName)
            //    });
            var result = await _decorated.HandleAsync(requestCommand, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
