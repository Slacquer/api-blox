﻿
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Attributes;
using Examples.Resources;

namespace Examples.CmdQueryHandlers
{
    [InjectableService]
    internal class ChildByIdRequestCommandHandler : ICommandHandler<ChildByIdRequest, HandlerResponse>
    {
        public Task<HandlerResponse> HandleAsync(ChildByIdRequest requestCommand, CancellationToken cancellationToken)
        {
            var ret = new HandlerResponse();
            
            return Task.FromResult(ret);
        }
    }
}
