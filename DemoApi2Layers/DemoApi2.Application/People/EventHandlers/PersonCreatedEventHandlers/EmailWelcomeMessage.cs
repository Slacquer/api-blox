#region -    Using Statements    -

using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using DemoApi2.Application.Contracts;
using DemoApi2.Domain.People.Events;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoApi2.Application.People.EventHandlers.PersonCreatedEventHandlers
{
    [InjectableService]
    public class EmailWelcomeMessage : IDomainEventHandler<PersonCreatedEvent>
    {
        #region -    Fields    -

        private readonly IEmailService _emailService;
        private readonly ILogger<EmailWelcomeMessage> _log;

        #endregion

        #region -    Constructors    -

        public EmailWelcomeMessage(
            ILoggerFactory loggerFactory,
            IEmailService emailService
        )
        {
            _log = loggerFactory.CreateLogger<EmailWelcomeMessage>();
            _emailService = emailService;
        }

        #endregion

        /// <inheritdoc />
        public Task HandleEventAsync(PersonCreatedEvent @event)
        {
            _log.LogInformation(() => $"Sending welcome message to new person: {@event.Person}");

            return _emailService.SendAsync(
                @event.Person.EmailAddress,
                "someEmailService@foo.com",
                "Welcome to the neighborhood",
                "Drop dead fatty!"
            );
        }
    }
}
