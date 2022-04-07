using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using Examples.Resources;

namespace Examples.CmdQueryHandlers
{
    [InjectableService]
    internal class ChildPostRequestCommandHandler : ICommandHandler<ChildPostRequest, HandlerResponse>
    {
        public Task<HandlerResponse> HandleAsync(ChildPostRequest requestCommand, CancellationToken cancellationToken)
        {
            var ret = new HandlerResponse
            {
                // it can not find a property that ends with "id", as it can't create a CreatedAtRoute.
                // if we don't return something the controller will complain.  It will also complain if
                Result = new {id = 1, Foo = "barrrr"}
            };

            return Task.FromResult(ret);
        }
    }
}
