using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
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
            var kids = new List<ChildResponse>();

            for (var i = 0; i < 10; i++)
                kids.Add(new ChildResponse {Age = i});

            ret.Result = kids;

            return Task.FromResult(ret);
        }
    }
}
