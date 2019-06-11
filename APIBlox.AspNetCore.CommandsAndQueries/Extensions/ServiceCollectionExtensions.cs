using System;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensionsCommandsAndQueries
    {
        /// <summary>
        ///     Decorates a REGISTERED command handler service.  Note that
        ///     decorations are executed in the order they are added.
        /// </summary>
        /// <remarks>
        ///     Your handler CAN implement more than one <see cref="ICommandHandler{TResult}"/> or
        ///     <seealso cref="ICommandHandler{TRequestQuery, TResult}"/>, and each will be wrapped with each decorator.
        /// </remarks>
        /// <typeparam name="THandler">The type of the t handler.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="decorators">The decorators.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddCommandHandlerDecoration<THandler>(
            this IServiceCollection services,
            ILoggerFactory loggerFactory,
            params Type[] decorators
        )
        {
            if (!decorators.Any())
                return services;

            var type = typeof(THandler);

            var qis = type.GetInterfaces()
                .Where(t =>
                    t.IsAssignableTo(typeof(ICommandHandler<>))
                    || t.IsAssignableTo(typeof(ICommandHandler<,>))
                ).ToList();

            if (!qis.Any())
                throw new ArgumentException(
                    $"The {type} must be an generic type of IQueryHandler<>, " +
                    "IQueryHandler<,>, ICommandHandler<> or ICommandHandler<,>) "
                );

            foreach (var decorator in decorators.Reverse())
            {
                foreach (var qi in qis)
                {
                    if (decorator.IsGenericType)
                    {
                        var p = qi.GetGenericArguments();
                        var decParams = decorator.MakeGenericType(p);
                        services.AddServiceDecoration(loggerFactory, qi, decParams);
                    }
                    else
                    {
                        services.AddServiceDecoration(loggerFactory, qi, decorator);
                    }
                }
            }

            return services;
        }

        /// <summary>
        ///     Decorates a REGISTERED query handler service.  Note that
        ///     decorations are executed in the order they are added.
        /// </summary>
        /// <remarks>
        ///     Your handler CAN implement more than one <see cref="IQueryHandler{TResult}"/> or
        ///     <seealso cref="IQueryHandler{TRequestQuery, TResult}"/>, and each will be wrapped with each decorator.
        /// </remarks>
        /// <typeparam name="THandler">The type of the t handler.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="decorators">The decorators.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddQueryHandlerDecoration<THandler>(
            this IServiceCollection services,
            ILoggerFactory loggerFactory,
            params Type[] decorators
        )
        {
            if (!decorators.Any())
                throw new ArgumentNullException(
                    nameof(decorators),
                    "Why would you decorate a handler with nothing?"
                );

            var ths = typeof(THandler);

            var qis = ths.GetInterfaces()
                .Where(t =>
                    t.IsAssignableTo(typeof(IQueryHandler<,>))
                    || t.IsAssignableTo(typeof(IQueryHandler<>))
                ).ToList();

            if (!qis.Any())
                throw new ArgumentException(
                    $"The {ths} must be an generic type of " +
                    "IQueryHandler<>, IQueryHandler<,>, ICommandHandler<> or ICommandHandler<,>) "
                );

            foreach (var decorator in decorators.Reverse())
            {
                foreach (var qi in qis)
                {
                    if (decorator.IsGenericType)
                    {
                        var p = qi.GetGenericArguments();
                        var decParams = decorator.MakeGenericType(p);
                        services.AddServiceDecoration(loggerFactory, qi, decParams);
                    }
                    else
                    {
                        services.AddServiceDecoration(loggerFactory, qi, decorator);
                    }
                }
            }

            return services;
        }
    }
}
