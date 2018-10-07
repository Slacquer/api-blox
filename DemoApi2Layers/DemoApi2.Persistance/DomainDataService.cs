#region -    Using Statements    -

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using DemoApi2.Domain.Contracts;

#endregion

namespace DemoApi2.Persistance
{
    namespace DemoApi.Persistance
    {
        [InjectableService]
        public class DomainDataService<TDomainModel, TId> : IDomainDataService<TDomainModel, TId>
            where TDomainModel : class, IDomainModel<TId>
            where TId : struct
        {
            #region -    Fields    -

            private static readonly List<TDomainModel> Repository = new List<TDomainModel>();
            private readonly IQueuedDomainEventsDispatcher _eventsDispatcher;

            #endregion

            #region -    Constructors    -

            // Not sure i like having the dispatcher in this thing, feels like it breaks the S in solid.
            public DomainDataService(IQueuedDomainEventsDispatcher eventsDispatcher)
            {
                _eventsDispatcher = eventsDispatcher;
            }

            #endregion

            /// <inheritdoc />
            public TDomainModel Create(TDomainModel poco)
            {
                Repository.Add(poco);

                return poco;
            }

            /// <inheritdoc />
            public void DeleteAll()
            {
                Repository.Clear();
            }

            /// <inheritdoc />
            public void DeleteById(TId id)
            {
                var existing = Repository.FirstOrDefault(m => m.Id.Equals(id));
                Repository.Remove(existing);
            }

            public Task<TDomainModel> GetByIdAsync(TId id, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return Task.FromResult(Repository.FirstOrDefault(m => m.Id.Equals(id)));
            }

            public IQueryable<TDomainModel> GeTRequestQueryable()
            {
                return Repository.AsQueryable();
            }

            /// <inheritdoc />
            public Task SaveChangesAsync(CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var events = Repository.SelectMany(m => m.Events).ToArray();

                // this would be done by the entity.
                foreach (var domainEvent in events)
                {
                    _eventsDispatcher.AddEvent(domainEvent);
                }

                return _eventsDispatcher.PublishEventsAsync();

                // actually save...
            }

            /// <inheritdoc />
            public TDomainModel Update(TDomainModel updatedPoco)
            {
                var existing = Repository.FirstOrDefault(m => m.Id.Equals(updatedPoco.Id));

                //if (existing is null)
                //    throw new NotFoundException(new ErrorResponseObject
                //    {
                //        Code = ErrorResponseCodes.ObjectNotFound,
                //        Message = $"Id {updatedPoco.Id} was not found.",
                //        Target = $"{typeof(TDomainModel).Name}.Id"
                //    });

                // Send it off/save it, etc.
                //existing.ModifiedOn = DateTimeOffset.Now;
                Repository.Remove(existing);
                Repository.Add(updatedPoco);

                return updatedPoco;
            }
        }
    }
}
