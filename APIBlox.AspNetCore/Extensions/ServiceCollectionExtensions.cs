#region -    Using Statements    -

using System;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Errors;

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
            if (!(alterAction is null))
                InternalHelpers.AlterRequestErrorObjectAction = alterAction;

            return services;
        }
    }
}
