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
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddDomainEventsDispatcher(this IServiceCollection services)
        {
            return services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
        }
    }
}
