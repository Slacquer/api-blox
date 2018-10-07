#region -    Using Statements    -

using APIBlox.NetCore;
using APIBlox.NetCore.Contracts;

#endregion

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the default domain events dispatcher to the service collection.
        ///     <para>
        ///         Designed for use with entities that manager their own events.
        ///     </para>
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddDefaultDomainEventsDispatcher(this IServiceCollection services)
        {
            return services.AddScoped<IDomainEventsDispatcher, DefaultDomainEventsDispatcher>();
        }

        /// <summary>
        ///     Adds the queued domain events dispatcher to the service collection.
        ///     <para>
        ///         Designed to be used with commands or either objects when entities do NOT manage their own events.
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
