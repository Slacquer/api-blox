using APIBlox.NetCore.Extensions;
using Examples.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;

namespace Examples
{
    internal class Startup
    {
        private const string SiteTitle = "APIBlox Example: Event Sourcing";

        private const string Version = "v1";
        private readonly IConfiguration _config;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration config, ILoggerFactory loggerFactory, IHostingEnvironment environment)
        {
            _config = config;
            _loggerFactory = loggerFactory;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddServerFaults()

                .AddEventStoreService<CosmosAggregate>()
                .AddCosmosDbRepository<CosmosAggregate>(_config)

                .AddEventStoreService<MongoAggregate>()
                .AddMongoDbRepository<MongoAggregate>(_config)

                .AddEventStoreService<RavenAggregate>()
                .AddRavenDbRepository<RavenAggregate>(_config)

                ;

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerExampleFeatures(SiteTitle, Version);
            
            BsonClassMap.RegisterClassMap<MongoAggregate>();
            BsonClassMap.RegisterClassMap<SomeValueAdded>();
            BsonClassMap.RegisterClassMap<SomeValueChanged>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //
            // Handle any and all server (500) errors with a defined structure.
            app.UseServerFaults();
            
            app.UseMvc();

            app.UseSwaggerExampleFeatures(SiteTitle, Version);
        }
    }
}
