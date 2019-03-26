using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Attributes;

namespace Examples.Queries
{
    [InjectableService]
    internal class NoInputsQueryHandler : IQueryHandler<IEnumerable<string>>
    {
        public Task<IEnumerable<string>> HandleAsync(CancellationToken cancellationToken)
        {
            var lst = new List<string> {"a", "b", "c"};

            return Task.FromResult<IEnumerable<string>>(lst);
        }
    }
}
