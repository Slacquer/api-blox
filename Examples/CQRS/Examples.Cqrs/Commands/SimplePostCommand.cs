using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Attributes;
using Examples.Resources;

namespace Examples.Commands
{
    [InjectableService]
    internal class SimplePostCommand : ICommandHandler<ExampleRequestObject>
    {
        public Task HandleAsync(ExampleRequestObject requestCommand, CancellationToken cancellationToken)
        {
            // This implementation does NOT return anything (other than the task object obviously).

            var foo = requestCommand.SomeValue;

            return Task.CompletedTask;
        }
    }
}
