using Examples.AggregateModels;
using Examples.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Examples
{
    internal class Startup
    {
        private const string SiteTitle = "APIBlox Example: Event Sourcing";

        private const string Version = "v1";
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddServerFaults()
                .AddEventStoreService<CosmosAggregate>()
                .AddCosmosDbRepository<CosmosAggregate>(_config)

                .AddEventStoreService<CosmosAggregate2>()
                .AddCosmosDbRepository<CosmosAggregate2>(_config)

                //.AddEventStoreService<MongoAggregate>()
                //.AddMongoDbRepository<MongoAggregate>(_config)
                //.AddEventStoreService<RavenAggregate>()
                //.AddRavenDbRepository<RavenAggregate>(_config)
                //.AddEventStoreService<EfCoreSqlAggregate>()
                //.AddEfCoreSqlRepository<EfCoreSqlAggregate>(_config)
                ;

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSwaggerExampleFeatures(SiteTitle, Version);

            //BsonClassMap.RegisterClassMap<MongoAggregate>();
            //BsonClassMap.RegisterClassMap<SomeValueAdded>();
            //BsonClassMap.RegisterClassMap<SomeValueChanged>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //
            // Handle any and all server (500) errors with a defined structure.
            app.UseServerFaults();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerExampleFeatures(SiteTitle, Version);
        }
    }
}
