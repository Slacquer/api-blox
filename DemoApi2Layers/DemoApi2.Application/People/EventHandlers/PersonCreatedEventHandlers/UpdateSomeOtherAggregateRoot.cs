#region -    Using Statements    -

using System;
using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using DemoApi2.Domain.People.Events;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoApi2.Application.People.EventHandlers.PersonCreatedEventHandlers
{
    [InjectableService]
    public class UpdateSomeOtherAggregateRoot : IDomainEventHandler<PersonCreatedEvent>
    {
        #region -    Fields    -

        private readonly ILogger<UpdateSomeOtherAggregateRoot> _log;

        #endregion

        #region -    Constructors    -

        public UpdateSomeOtherAggregateRoot(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<UpdateSomeOtherAggregateRoot>();
        }

        #endregion

        /// <inheritdoc />
        public async Task HandleEventAsync(PersonCreatedEvent @event)
        {
            _log.LogInformation(
                () => "Starting update for some other aggregate root using the " +
                      $"created person event with id: {@event.AggregateId}"
            );
            await Task.Delay(new Random().Next(500, 2000)).ConfigureAwait(false);

            _log.LogInformation(
                () => "Updated some other aggregate root using the " +
                      $"created person event with id: {@event.AggregateId}"
            );
        }
    }
}
