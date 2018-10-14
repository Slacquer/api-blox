using System;
using APIBlox.AspNetCore.Types.Errors;
using DemoApi2.Presentation.People;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace DemoApi2.Presentation
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        private readonly ILoggerFactory _loggerFactory;

        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment env)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddInjectableServices(_loggerFactory,
                    new[] {"DemoApi"}, //, "APIBlox" },
                    new[]
                    {
                        @"..\DemoApi2.Persistance\bin\Debug\netcoreapp2.1",
                        @"..\DemoApi2.Infrastructure\bin\Debug\netcoreapp2.1"
                    }
                )
                .AddDefaultDomainEventsDispatcher()
                .AddQueuedDomainEventsDispatcher()
                .AddAlterRequestErrorObject(e =>
                    {
                        e.Type = "about:blank2";

                        e.Errors.Add(new DynamicErrorObject("OMG", "Altered!"));
                    }
                )
                .AddMvc()
                //.AddEnsureResponseResultActionFilter(o => new {Resources = o})
                .AddEnsurePaginationResultActionFilter(25)
                .AddValidateResourceActionFilter()
                .AddPopulateRequestObjectActionFilter()
                .AddPopulateGenericRequestObjectActionFilter()
                .AddOperationCancelledExceptionFilter()
                .AddDynamicControllersFeature(c => PeopleConfiguration.Configure(services, c))
                .AddConsumesProducesJsonResourceResultFilters()
                .AddRouteTokensConvention(_configuration, _env)
                .AddFluentValidation()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "My API", Version = "v1"}); });

            //services.AddAuthorization();

            //services.AddMvc(o =>
            //    {
            //        // All endpoints need authorization using our custom authorization filter
            //        o.Filters.Add(new ProblemResultAuthorizationFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            //    }
            //);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseServerFaults();

            app.UseAuthentication();

            app.UseLameApiExplorer();
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            app.UseMvc();
        }
    }
}
