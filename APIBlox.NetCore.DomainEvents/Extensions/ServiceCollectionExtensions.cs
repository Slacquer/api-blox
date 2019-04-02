using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensionsDomainEvents
    {
        /// <summary>
        ///     Adds the default domain events dispatcher to the service collection.
        ///     <para>
        ///         Designed for use with entities that manager their own events.
        ///     </para>
        ///     <para>
        ///         This is not mutually exclusive with <see cref="AddQueuedDomainEventsDispatcher" />.  Both can be used.
        ///     </para>
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddDomainEventsDispatcher(this IServiceCollection services)
        {
            return services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
        }

        /// <summary>
        ///     Adds the queued domain events dispatcher to the service collection.
        ///     <para>
        ///         Designed to be used with commands or objects when entities do NOT manage their own events.
        ///     </para>
        ///     <para>
        ///         This is not mutually exclusive with <see cref="AddDomainEventsDispatcher" />.  Both can be used.
        ///     </para>
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddQueuedDomainEventsDispatcher(this IServiceCollection services)
        {
            return services.AddScoped<IQueuedDomainEventsDispatcher, QueuedDomainEventsDispatcher>();
        }
    }
}
