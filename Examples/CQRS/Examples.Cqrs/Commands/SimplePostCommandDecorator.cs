using System;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using Examples.Resources;

namespace Examples.Commands
{
    // Not needed as we must explicitly wrap decorators.
    //[InjectableService]
    internal class SimplePostCommandDecorator : ICommandHandler<ExampleRequestObject>
    {
        private readonly ICommandHandler<ExampleRequestObject> _thingWeAreDecorating;

        public SimplePostCommandDecorator(ICommandHandler<ExampleRequestObject> thingWeAreDecorating)
        {
            _thingWeAreDecorating = thingWeAreDecorating;
        }

        public Task HandleAsync(ExampleRequestObject requestCommand, CancellationToken cancellationToken)
        {
            // Do some kind of test or perhaps domain validation, prior to letting the actual command handler deal with it.
            _thingWeAreDecorating.HandleAsync(requestCommand, cancellationToken);
            
            // Do something after the handler has dealt with it.
            
            return Task.CompletedTask;
        }
    }
}
