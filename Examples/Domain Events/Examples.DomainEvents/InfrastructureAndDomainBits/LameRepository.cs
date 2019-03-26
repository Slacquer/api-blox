using System.Collections.Generic;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using Examples.Contracts;
using Examples.DomainModels;

namespace Examples
{
    [InjectableService]
    internal class LameRepository : ILameRepository
    {
        private static readonly Dictionary<int, DomainObject> Models = new Dictionary<int, DomainObject>();
        private readonly IDomainEventsDispatcher _dispatcher;

        public LameRepository(IDomainEventsDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void AddDomainObject(DomainObject domainObject)
        {
            if (Models.ContainsKey(domainObject.Id))
                Models.Remove(domainObject.Id);

            Models.Add(domainObject.Id, domainObject);
        }

        public void SaveChanges()
        {
            var lst = new List<IDomainEvent>();

            foreach (var obj in Models.Values)
                lst.AddRange(obj.Events);

            _dispatcher.PublishEventsAsync(lst.ToArray());
        }
    }
}
