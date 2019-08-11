using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using Examples.Resources;

namespace Examples.Commands
{
    [InjectableService]
    internal class SimplePostCommand : ICommandHandler<ExampleRequestObject, HandlerResponse>
    {
        public Task<HandlerResponse> HandleAsync(ExampleRequestObject requestCommand, CancellationToken cancellationToken)
        {
            // This implementation does NOT return anything (other than the task object obviously).

            var res = new HandlerResponse().SetErrorTo400BadRequest("u blow");
            res.Result = requestCommand.SomeValue;

            return Task.FromResult(res);
        }
    }
}
