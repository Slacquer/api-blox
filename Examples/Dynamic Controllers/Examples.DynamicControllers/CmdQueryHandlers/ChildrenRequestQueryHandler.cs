using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Attributes;
using Examples.Resources;

namespace Examples.CmdQueryHandlers
{
    [InjectableService]
    internal class ChildrenRequestQueryHandler : IQueryHandler<ChildrenRequest, HandlerResponse>
    {
        public Task<HandlerResponse> HandleAsync(ChildrenRequest query, CancellationToken cancellationToken)
        {
            var ret = new HandlerResponse();

            // if we do not return something then the controller will get mad at us!

            return Task.FromResult(ret);
        }
    }
}
