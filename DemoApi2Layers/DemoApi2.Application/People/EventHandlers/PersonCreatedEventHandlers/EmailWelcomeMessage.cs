using System;
using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using DemoApi2.Application.Contracts;
using DemoApi2.Domain.People.Events;
using Microsoft.Extensions.Logging;

namespace DemoApi2.Application.People.EventHandlers.PersonCreatedEventHandlers
{
    [InjectableService]
    public class EmailWelcomeMessage : IDomainEventHandler<PersonCreatedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailWelcomeMessage> _log;

        public EmailWelcomeMessage(
            ILoggerFactory loggerFactory,
            IEmailService emailService
        )
        {
            _log = loggerFactory.CreateLogger<EmailWelcomeMessage>();
            _emailService = emailService;
        }

        /// <inheritdoc />
        public async Task HandleEventAsync(PersonCreatedEvent domainEvent)
        {
            _log.LogInformation(() => $"Sending welcome message to new person: {domainEvent.Person}");

            await Task.Delay(new Random().Next(250, 5000)).ConfigureAwait(false);

            await _emailService.SendAsync(
                domainEvent.Person.EmailAddress,
                "someEmailService@foo.com",
                "Welcome to the neighborhood",
                "Drop dead fatty!"
            ).ConfigureAwait(false);
        }
    }
}
