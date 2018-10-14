//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using APIBlox.AspNetCore.Contracts;
//using APIBlox.NetCore.Attributes;
//using DemoApi2.Domain.Contracts;
//using DemoApi2.Domain.People;
//using Newtonsoft.Json;

//// ReSharper disable once CheckNamespace
//namespace DemoApi2.Application.People.Queries
//{
//    [InjectableService]
//    public class GetPersonByFullNameQueryHandler : IQueryHandler<PersonQueryByFullname, PersonResponse>
//    {
//        private readonly IDomainDataService<PersonDomainModel, int> _dataService;

//        public GetPersonByFullNameQueryHandler(IDomainDataService<PersonDomainModel, int> dataService)
//        {
//            _dataService = dataService;
//        }

//        /// <inheritdoc />
//        public Task<PersonResponse> HandleAsync(PersonQueryByFullname query, CancellationToken cancellationToken)
//        {
//            var result = _dataService.GeTRequestQueryable().FirstOrDefault(
//                m =>
//                    m.ToString().Equals(query.FullName, StringComparison.InvariantCultureIgnoreCase)
//            );

//            return result is null
//                ? null
//                : Task.FromResult(JsonConvert.DeserializeObject<PersonResponse>(JsonConvert.SerializeObject(result)));
//        }
//    }
//}
