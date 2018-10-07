#region -    Using Statements    -

using DemoApi2.Presentation.People;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

#endregion

namespace DemoApi2.Presentation
{
    public class Startup
    {
        #region -    Fields    -

        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        private readonly ILoggerFactory _loggerFactory;

        #endregion

        #region -    Constructors    -

        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment env)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;
            _env = env;
        }

        #endregion
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddInjectableServices(
                    new[] {"DemoApi"}, //, "APIBlox" },
                    new[]
                    {
                        @"..\DemoApi2.Persistance\bin\Debug\netcoreapp2.1",
                        @"..\DemoApi2.Infrastructure\bin\Debug\netcoreapp2.1"
                    }
                )
                .AddDefaultDomainEventsDispatcher()
                .AddMvc()
                .AddEnsureResponseResultActionFilter(o => new {Resources = o})
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseServerFaults();
            app.UseLameApiExplorer();
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            app.UseMvc();
        }
    }
}
