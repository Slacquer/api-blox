using System;
using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.EventStore.MongoDb;
using APIBlox.NetCore.EventStore.MongoDb.Options;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensionsMongoDb
    {
        /// <summary>
        ///     Adds the cosmos database repository for use with the <see cref="IEventStoreService"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="configSection">The configuration section.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="ArgumentException">In order to use the {nameof(MongoDbOptions)} you " +
        ///                     $"will need to have an {configSection}</exception>
        public static IServiceCollection AddMongoDbRepository<TModel>(this IServiceCollection services, IConfiguration configuration, string configSection = "CosmosDbOptions")
            where TModel : class
        {
            var config = configuration.GetSection(configSection);

            var es = config.Get<MongoDbOptions>();

            if (es is null)
                throw new ArgumentException(
                    $"In order to use the {nameof(MongoDbOptions)} you " +
                    $"will need to have an {configSection} configuration entry."
                );

            services.Configure<MongoDbOptions>(config);

            services.AddSingleton(x => new CollectionContext(Options.Options.Create(es)));

            services.AddScoped<IEventStoreRepository, MongoDbRepository<TModel>>(x =>
            {
                var ret = new MongoDbRepository<TModel>(x.GetRequiredService<CollectionContext>())
                {
                    JsonSettings = new CamelCaseSettings
                    {
                        ContractResolver = new CamelCasePopulateNonPublicSettersContractResolver()
                    }
                };

                return ret;
            });

            return services;
        }
    }
}
