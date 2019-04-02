using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Attributes;
using Examples.Resources;

namespace Examples.CmdQueryHandlers
{
    [InjectableService]
    internal class ChildPatchRequestCommandHandler : ICommandHandler<ChildPatchRequest, HandlerResponse>
    {
        public Task<HandlerResponse> HandleAsync(ChildPatchRequest requestCommand, CancellationToken cancellationToken)
        {
            var ret = new HandlerResponse();

            return Task.FromResult(ret);
        }
    }
}
