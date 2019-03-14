using System;
using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;
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

        
        public static IServiceCollection AddMongoDbRepository<TModel>(this IServiceCollection services, IConfiguration configuration, string configSection = "CosmosDbOptions")
            where TModel : class
        {
            //var config = configuration.GetSection(configSection);

            //var es = config.Get<CosmosDbOptions>();

            //if (es is null)
            //    throw new ArgumentException(
            //        $"In order to use the {nameof(CosmosDbOptions)} you " +
            //        $"will need to have an {configSection} configuration entry."
            //    );

            //services.Configure<CosmosDbOptions>(config);

            //services.AddSingleton<IDocumentClient>(x => new DocumentClient(new Uri(es.Endpoint), es.Key));

            //services.AddScoped<IEventStoreRepository, CosmosDbRepository<TModel>>(x =>
            //{
            //    var opt = Options.Options.Create(es);
            //    var ret = new CosmosDbRepository<TModel>(x.GetRequiredService<IDocumentClient>(), opt)
            //    {
            //        JsonSettings = new CamelCaseSettings
            //        {
            //            ContractResolver = new CamelCasePopulateNonPublicSettersContractResolver()
            //        }
            //    };

            //    return ret;
            //});

            return services;
        }
    }
}
