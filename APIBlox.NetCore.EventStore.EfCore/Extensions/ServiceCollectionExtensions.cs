using System;
using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Options;
using APIBlox.NetCore.Types.JsonBits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ServiceCollectionExtensionsEfCore.
    /// </summary>
    public static class ServiceCollectionExtensionsEfCore
    {
        /// <summary>
        ///     Adds the ef core SQL repository for <see cref="IEventStoreService{TModel}" />.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serializerSettings">The serializer settings.</param>
        /// <param name="configSection">The configuration section.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="System.ArgumentException">
        ///     In order to use the {nameof(SqlServerOptions)} you " +
        ///     $"will need to have an {configSection}
        /// </exception>
        public static IServiceCollection AddEfCoreSqlRepository<TModel>(this IServiceCollection services, IConfiguration configuration,
            JsonSerializerSettings serializerSettings = null, string configSection = "EfCoreSqlOptions"
        )
            where TModel : class
        {
            var settings = serializerSettings ?? new CamelCaseSettings();
            settings.ContractResolver = new CamelCasePopulateNonPublicSettersContractResolver();
            settings.Converters.Add(new StringEnumConverter());

            var config = configuration.GetSection(configSection);

            var es = config.Get<EfCoreSqlOptions>();

            if (es is null)
                throw new ArgumentException(
                    $"In order to use the {nameof(EfCoreSqlOptions)} you " +
                    $"will need to have an {configSection} configuration entry."
                );

            services.AddDbContext<EventStoreDbContext>(o =>
                {
                    o.UseSqlServer(es.CnnString);
                    o.EnableDetailedErrors(es.EnableDetailedErrors);
                    o.EnableSensitiveDataLogging(es.EnableSensitiveDataLogging);

                    if (es.ConfigureWarnings)
                        o.ConfigureWarnings(w =>
                            w.Throw(
                                //RelationalEventId.QueryClientEvaluationWarning,
                               // RelationalEventId.QueryPossibleExceptionWithAggregateOperator,
                                RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning
                            )
                        );
                }
            );

            services.AddScoped<IEventStoreRepository<TModel>, EfCoreSqlRepository<TModel>>(
                x =>
                {
                    var ctx = x.GetRequiredService<EventStoreDbContext>();

                    ctx.Database.Migrate();

                    return new EfCoreSqlRepository<TModel>(ctx,
                        x.GetRequiredService<IEventSourcedJsonSerializerSettings>()
                    );
                }
            );

            return services;
        }
    }
}
