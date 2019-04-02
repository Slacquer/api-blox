using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using Microsoft.Extensions.Logging;

namespace Examples.EventBits
{
    [InjectableService]
    internal class RequestObjectCreatedEventHandler : IDomainEventHandler<RequestObjectCreatedEvent>
    {
        private readonly ILogger<RequestObjectCreatedEventHandler> _log;

        public RequestObjectCreatedEventHandler(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<RequestObjectCreatedEventHandler>();
        }

        public Task HandleEventAsync(RequestObjectCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _log.LogInformation(() =>
                "------------------\n\nHandling created event.  Its special value " +
                $"is {domainEvent.SomeOtherDomainSpecificEventValueNeededForConsumption}, the " +
                $"value that was used during request was {domainEvent.TheValueThatWasCreated}\n\n-------------\n\n"
            );

            // Not actually doing anything outside of a log entry.
            // Just imagine I am part of a different aggregate root.

            return Task.CompletedTask;
        }
    }
}
