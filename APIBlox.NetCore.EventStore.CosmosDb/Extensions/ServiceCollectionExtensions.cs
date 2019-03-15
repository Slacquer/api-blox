using System;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Options;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace APIBlox.NetCore.Extensions
{
    /// <summary>
    ///     Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensionsCosmosDb
    {
        /// <summary>
        ///     Adds the cosmos database repository for use with the <see cref="IEventStoreService{TModel}"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serializerSettings">The serializer settings.</param>
        /// <param name="configSection">The configuration section.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="ArgumentException">In order to use the {nameof(CosmosDbOptions)} you " +
        ///                     $"will need to have an {configSection}</exception>
        public static IServiceCollection AddCosmosDbRepository<TModel>(this IServiceCollection services, IConfiguration configuration,
            JsonSerializerSettings serializerSettings = null, string configSection = "CosmosDbOptions")
            where TModel : class
        {
            var config = configuration.GetSection(configSection);

            var es = config.Get<CosmosDbOptions>();

            if (es is null)
                throw new ArgumentException(
                    $"In order to use the {nameof(CosmosDbOptions)} you " +
                    $"will need to have an {configSection} configuration entry."
                );

            services.Configure<CosmosDbOptions>(config);

            services.AddSingleton<IDocumentClient>(x => new DocumentClient(new Uri(es.Endpoint), es.Key));

            var settings = serializerSettings;
            services.AddScoped<IEventStoreRepository<TModel>, CosmosDbRepository<TModel>>(x =>
            {
                var opt = Microsoft.Extensions.Options.Options.Create(es);
                var ret = new CosmosDbRepository<TModel>(x.GetRequiredService<IDocumentClient>(),
                    settings ?? new CamelCaseSettings { ContractResolver = new CamelCasePopulateNonPublicSettersContractResolver() },
                    opt
                );

                return ret;
            });

            return services;
        }
    }
}
