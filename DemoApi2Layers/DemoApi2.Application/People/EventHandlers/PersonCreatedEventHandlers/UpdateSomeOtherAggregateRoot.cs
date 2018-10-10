using System;
using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using DemoApi2.Domain.People.Events;
using Microsoft.Extensions.Logging;

namespace DemoApi2.Application.People.EventHandlers.PersonCreatedEventHandlers
{
    [InjectableService]
    public class UpdateSomeOtherAggregateRoot : IDomainEventHandler<PersonCreatedEvent>
    {
        private readonly ILogger<UpdateSomeOtherAggregateRoot> _log;

        public UpdateSomeOtherAggregateRoot(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<UpdateSomeOtherAggregateRoot>();
        }

        /// <inheritdoc />
        public async Task HandleEventAsync(PersonCreatedEvent domainEvent)
        {
            _log.LogInformation(
                () => "Starting update for some other aggregate root using the " +
                      $"created person event with id: {domainEvent.AggregateId}"
            );

            await Task.Delay(new Random().Next(1000, 3000)).ConfigureAwait(false);

            _log.LogInformation(
                () => "Updated some other aggregate root using the " +
                      $"created person event with id: {domainEvent.AggregateId}"
            );
        }
    }
}
