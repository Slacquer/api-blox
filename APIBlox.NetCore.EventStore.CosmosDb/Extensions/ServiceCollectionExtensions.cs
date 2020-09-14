using System;
using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Options;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensionsCosmosDb
    {
        /// <summary>
        ///     Adds the cosmos database repository for use with the <see cref="IEventStoreService{TModel}" />.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="configSection">The configuration section.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="ArgumentException">
        ///     In order to use the {nameof(CosmosDbOptions)} you " +
        ///     $"will need to have an {configSection}
        /// </exception>
        public static IServiceCollection AddCosmosDbRepository<TModel>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configSection = "CosmosDbOptions"
        )
            where TModel : class
        {

            var config = configuration.GetSection(configSection);

            var es = config.Get<CosmosDbOptions>();

            if (es is null)
                throw new ArgumentException($"Configuration section {configSection} not found.");

            services.Configure<CosmosDbOptions>(config);

            services.AddSingleton<IDocumentClient>(sp =>
            {
                var client = new DocumentClient(new Uri(es.Endpoint),
                    es.Key,
                    sp.GetRequiredService<IEventSourcedJsonSerializerSettings>().Settings,
                    es.ConnectionPolicy
                );
            
                // Microsoft Suggested...
                // https://docs.microsoft.com/en-us/azure/cosmos-db/performance-tips
                client.OpenAsync();

                return client;
            });
            services.AddScoped<IEventStoreRepository<TModel>, CosmosDbRepository<TModel>>();

            return services;
        }
    }
}
