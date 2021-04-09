using System;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Types;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensionsAspNetCore
    {
        /// <summary>
        ///     Adds the server faults middleware to the pipeline.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddServerFaults(this IServiceCollection services)
        {
            services.TryAddSingleton<IActionResultExecutor<ProblemResult>, ObjectResultExecutor>();

            return services;
        }

        /// <summary>
        ///     Adds a callback mechanism allowing you to modify a fully built
        ///     <see cref="RequestErrorObject" /> just prior to being sent.
        ///     <para>
        ///         This is specifically designed to allow editing the
        ///         <see cref="RequestErrorObject.Type" /> value for a more specific URL.
        ///     </para>
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="alterAction">The alter action.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddAlterRequestErrorObject(this IServiceCollection services, Action<RequestErrorObject> alterAction)
        {
            if (alterAction is not null)
                RequestErrorObject.RequestErrorObjectAction = alterAction;

            return services;
        }
    }
}
