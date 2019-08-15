using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using Examples.Resources;

namespace Examples.CmdQueryHandlers
{
    [InjectableService]
    internal class ChildByIdRequestCommandHandler : ICommandHandler<ChildByIdRequest, HandlerResponse>
    {
        public Task<HandlerResponse> HandleAsync(ChildByIdRequest requestCommand, CancellationToken cancellationToken)
        {
            var ret = new HandlerResponse();

            ret.Result = new ChildResponse {Age = 4};

            return Task.FromResult(ret);
        }
    }
}
