using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;

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
        /// <param name="useCompression">When true, all data stored will be GZipped first.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddEventStoreService<TModel>(this IServiceCollection services, bool useCompression = false)
            where TModel : class
        {
            return services.AddScoped<IEventStoreService<TModel>, EventStoreService<TModel>>(s =>
                {
                    var repo = s.GetRequiredService<IEventStoreRepository<TModel>>();
                    return new EventStoreService<TModel>(repo, useCompression);
                }
            );
        }

        /// <summary>
        ///     Adds the read only event store service.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model (perhaps your ddd aggregate).</typeparam>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        /// <param name="useCompression">When true, all data stored will be GZipped first.</param>
        public static IServiceCollection AddReadOnlyEventStoreService<TModel>(this IServiceCollection services, bool useCompression = false)
            where TModel : class
        {
            return services.AddScoped<IReadOnlyEventStoreService<TModel>, ReadOnlyEventStoreService<TModel>>(s =>
                {
                    var repo = s.GetRequiredService<IEventStoreRepository<TModel>>();
                    return new ReadOnlyEventStoreService<TModel>(repo, useCompression);
                }
            );
        }
    }
}
