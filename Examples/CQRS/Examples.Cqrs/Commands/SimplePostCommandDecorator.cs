using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Contracts;
using Examples.Resources;

namespace Examples.Commands
{
    // Not needed as we must explicitly wrap decorators.
    //[InjectableService]
    internal class SimplePostCommandDecorator : ICommandHandler<ExampleRequestObject, HandlerResponse>
    {
        private readonly ICommandHandler<ExampleRequestObject, HandlerResponse> _thingWeAreDecorating;

        public SimplePostCommandDecorator(ICommandHandler<ExampleRequestObject, HandlerResponse> thingWeAreDecorating)
        {
            _thingWeAreDecorating = thingWeAreDecorating;
        }

        public async Task<HandlerResponse> HandleAsync(ExampleRequestObject requestCommand, CancellationToken cancellationToken)
        {
            // Do some kind of test or perhaps domain validation, prior to letting the actual command handler deal with it.
            // If validation failed, we could short circuit the process by NOT calling the decorated handler.
            var ret = await _thingWeAreDecorating.HandleAsync(requestCommand, cancellationToken);

            // Do something after the handler has dealt with it.

            return ret;
        }

    }
}
