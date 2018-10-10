using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoApi2.Domain.Contracts
{
    public interface IDomainDataService<TDomainModel, in TId>
        where TDomainModel : IDomainModel<TId>
    {
        TDomainModel Create(TDomainModel poco);

        void DeleteAll();

        void DeleteById(TId id);

        Task<TDomainModel> GetByIdAsync(TId id, CancellationToken cancellationToken);

        IQueryable<TDomainModel> GeTRequestQueryable();

        Task SaveChangesAsync(CancellationToken cancellationToken);

        TDomainModel Update(TDomainModel updatedPoco);
    }
}
