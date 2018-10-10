using System;
using Microsoft.Extensions.DependencyInjection;

namespace APIBlox.NetCore.Attributes
{
    /// <summary>
    ///     Attribute that should be applied to any and all services that are dependencies.
    ///     <para>
    ///         Service LifeTime defaults to <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped" />
    ///     </para>
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectableServiceAttribute : Attribute
    {
        /// <summary>
        ///     Gets or sets the service lifetime.  Defaults to
        ///     <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped" />
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
    }
}
