using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;

namespace Examples.Queries
{
    [InjectableService]
    internal class RequiresInputQueryQueryHandler : IQueryHandler<int, int>
    {
        public Task<int> HandleAsync(int query, CancellationToken cancellationToken)
        {
            return Task.FromResult(query * 100);
        }
    }
}
