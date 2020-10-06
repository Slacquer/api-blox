using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Types.JsonBits;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensionsEventStore
    {
        /// <summary>
        ///     Adds the event store service.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model (perhaps your ddd aggregate).</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="contractResolver">
        ///     The contract resolver.  Uses the <see cref="PopulateNonPublicSettersContractResolver" /> if null.
        /// </param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddEventStoreService<TModel>(this IServiceCollection services, IContractResolver contractResolver = null)
            where TModel : class
        {
            services.AddSingleton<IEventStoreJsonSerializerSettings>(sp => new EventSourcedJsonSerializerSettings(contractResolver));

            return services.AddScoped<IEventStoreService<TModel>, EventStoreService<TModel>>();
        }

        /// <summary>
        ///     Adds the read only event store service.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model (perhaps your ddd aggregate).</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="contractResolver">
        ///     The contract resolver.  Uses the <see cref="PopulateNonPublicSettersContractResolver" /> if null.
        /// </param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddReadOnlyEventStoreService<TModel>(this IServiceCollection services, IContractResolver contractResolver = null
        )
            where TModel : class
        {
            services.AddSingleton<IEventStoreJsonSerializerSettings>(sp => new EventSourcedJsonSerializerSettings(contractResolver));

            return services.AddScoped<IReadOnlyEventStoreService<TModel>, ReadOnlyEventStoreService<TModel>>();
        }
    }
}
