#region -    Using Statements    -

using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Controllers;
using DemoApi2.Application.Locations;
using DemoApi2.Application.Locations.Queries;
using DemoApi2.Application.People;
using DemoApi2.Application.People.Commands;
using DemoApi2.Application.People.Queries;
using DemoApi2.Presentation.Controllers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

#endregion

namespace DemoApi2.Presentation.People
{
    public static class PeopleConfiguration
    {
        public static IServiceCollection Configure(
            IServiceCollection services,
            IDynamicControllerConfigurations config
        )
        {
            //
            // Where we tie Resource/Handlers/Controllers together...
            //

            //// Controllers
            var peopleRoutes = new[] { "api/companies/{companyId:int}/departments/{departmentId:double}/people" };
            //var locationRoutes = new[] { "{personId:int}/locations" };

            //config.AddController<PersonQueryById>(peopleRoutes, "people", typeof(QueryByIdController<>))
            //    .AddSubController<LocationQuery>(typeof(int), locationRoutes, "locations", typeof(QueryByIdController<>))
            //    ;

            ////config.AddController<PersonQuery>(peopleRoutes, "people", typeof(QueryController<>))
            ////    ;//.AddSubController<LocationQuery>(typeof(int), locationRoutes, "locations", typeof(QueryByIdController<>));

            //var noRouteParamsRoute = new[] { "api/serSettings" };
            //config.AddController<PersonNoRouteParamsJustAOrderByQuery, PersonResponse, int>(noRouteParamsRoute, "people", typeof(QueryController<,,>))
            //    ;//.AddSubController<LocationQuery>(typeof(int), locationRoutes, "locations", typeof(QueryByIdController<>));


            //var byIdRoute = new[] { "api/users/{userId:int}/userSettings/{id:int}" };

            //config.AddController<DeletePersonByIdCommand>(byIdRoute,
            //    "userSettings",
            //    typeof(DefaultDeleteByController<>)
            //);

            //var byKeyRoute = new[] { "api/users/{userId:int}/userSettings/{key}" };
            //config.AddController<DeletePersonByKeyCommand>(byKeyRoute,
            //    "userSettings",
            //    typeof(DefaultDeleteByController<>)
            //);
            config.AddController<PersonCommand, PersonResponse, int>(peopleRoutes, typeof(DynamicPostController<,,>));

            ////Resource validation
            //services.TryAddTransient<IValidator<PersonResource>, PersonRequestResourceValidator>();

            ////
            //// Add all CQRS elements for domain objects, these are added by way of the services
            //// already added, so it's important that AddInjectableServices has been done first.
            //services
            //   .AddQueryHandlerDecoration<GetAllPeopleQueryHandler>(typeof(MetricsQueryHandlerDecorator<>))
            //   .AddQueryHandlerDecoration<GetPersonByIdQueryHandler>(typeof(MetricsQueryHandlerDecorator<,>))

            //   .AddCommandHandlerDecoration<CreateNewPersonCommandHandler>(
            //       typeof(MetricsCommandHandlerDecorator<,>),
            //       typeof(TransactionScopeCommandHandlerDecorator<,>),
            //       typeof(ValidateCreateNewPersonCommandHandlerDecorator));

            return services;
        }
    }
}
