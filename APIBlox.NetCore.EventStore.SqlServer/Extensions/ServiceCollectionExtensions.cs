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
    public static class ServiceCollectionExtensionsSqlServer
    {
        /// <summary>
        ///     Adds the raven database repository for use with the <see cref="IEventStoreService{TModel}"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serializerSettings">The serializer settings.</param>
        /// <param name="configSection">The configuration section.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="ArgumentException">In order to use the {nameof(MongoDbOptions)} you " +
        ///                     $"will need to have an {configSection}</exception>
        public static IServiceCollection AddSqlServerRepository<TModel>(this IServiceCollection services, IConfiguration configuration,
            JsonSerializerSettings serializerSettings = null, string configSection = "SqlServerOptions")
            where TModel : class
        {
            var settings = serializerSettings ?? new CamelCaseSettings();
            settings.ContractResolver = new CamelCasePopulateNonPublicSettersContractResolver();
            settings.Converters.Add(new StringEnumConverter());

            var config = configuration.GetSection(configSection);

            services.Configure<SqlServerOptions>(config);

            services.AddSingleton(x =>
            {
                var es = config.Get<SqlServerOptions>();

                if (es is null)
                    throw new ArgumentException(
                        $"In order to use the {nameof(SqlServerOptions)} you " +
                        $"will need to have an {configSection} configuration entry."
                    );

                return new SqlDbContext(es);
            });

            services.AddScoped<IEventStoreRepository<TModel>, SqlServerRepository<TModel>>(
                x => new SqlServerRepository<TModel>(x.GetRequiredService<SqlDbContext>())
            );

            return services;
        }
    }
}
