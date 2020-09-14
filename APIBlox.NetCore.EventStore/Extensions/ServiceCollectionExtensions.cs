﻿using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Types.JsonBits;
using Newtonsoft.Json;

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
        /// <param name="settings">
        ///     The settings.  Uses the <see cref="PopulateNonPublicSettersContractResolver" /> if no settings
        ///     are provided.
        /// </param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddEventStoreService<TModel>(this IServiceCollection services, JsonSerializerSettings settings = null)
            where TModel : class
        {
            services.AddSingleton<IEventStoreJsonSerializerSettings>(sp => new EventSourcedJsonSerializerSettings(settings));

            return services.AddScoped<IEventStoreService<TModel>, EventStoreService<TModel>>();
        }

        /// <summary>
        ///     Adds the read only event store service.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model (perhaps your ddd aggregate).</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="settings">
        ///     The settings.  Uses the <see cref="PopulateNonPublicSettersContractResolver" /> if no settings
        ///     are provided.
        /// </param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddReadOnlyEventStoreService<TModel>(this IServiceCollection services, JsonSerializerSettings settings = null
        )
            where TModel : class
        {
            services.AddSingleton<IEventStoreJsonSerializerSettings>(sp => new EventSourcedJsonSerializerSettings(settings));

            return services.AddScoped<IReadOnlyEventStoreService<TModel>, ReadOnlyEventStoreService<TModel>>();
        }
    }
}
