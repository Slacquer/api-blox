﻿using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using Examples.Resources;

namespace Examples.CmdQueryHandlers
{
    [InjectableService]
    internal class ChildPutRequestCommandHandler : ICommandHandler<ChildPutRequest, HandlerResponse>
    {
        public Task<HandlerResponse> HandleAsync(ChildPutRequest requestCommand, CancellationToken cancellationToken)
        {
            var ret = new HandlerResponse();

            return Task.FromResult(ret);
        }
    }
}
