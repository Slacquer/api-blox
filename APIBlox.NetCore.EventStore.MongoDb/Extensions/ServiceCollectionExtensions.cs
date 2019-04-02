using System;
using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Options;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensionsMongoDb
    {
        /// <summary>
        ///     Adds the cosmos database repository for use with the <see cref="IEventStoreService{TModel}" />.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serializerSettings">The serializer settings.</param>
        /// <param name="configSection">The configuration section.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="ArgumentException">
        ///     In order to use the {nameof(MongoDbOptions)} you " +
        ///     $"will need to have an {configSection}
        /// </exception>
        public static IServiceCollection AddMongoDbRepository<TModel>(this IServiceCollection services, IConfiguration configuration,
            JsonSerializerSettings serializerSettings = null, string configSection = "MongoDbOptions"
        )
            where TModel : class
        {
            var settings = serializerSettings ?? new CamelCaseSettings();
            settings.Converters.Add(new StringEnumConverter());

            var config = configuration.GetSection(configSection);

            services.Configure<MongoDbOptions>(config);

            services.AddSingleton(x =>
                {
                    var es = config.Get<MongoDbOptions>();

                    if (es is null)
                        throw new ArgumentException(
                            $"In order to use the {nameof(MongoDbOptions)} you " +
                            $"will need to have an {configSection} configuration entry."
                        );

                    return new CollectionContext(es);
                }
            );

            services.AddScoped<IEventStoreRepository<TModel>, MongoDbRepository<TModel>>(x =>
                {
                    var ret = new MongoDbRepository<TModel>(x.GetRequiredService<CollectionContext>(),
                        settings
                    );

                    return ret;
                }
            );

            return services;
        }
    }
}
