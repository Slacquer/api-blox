using System;
using System.Collections.Generic;
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
        ///     <seealso cref="ICommandHandler{TRequestQuery, TResult}"/>, and each will be wrapped
        ///     with each decorator, this also holds true with the decorator itself.
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
            var qis = type.GetCommandHandlerList();

            foreach (var decorator in decorators.Reverse())
            {
                foreach (var qi in qis)
                {
                    if (decorator.IsGenericType)
                        services.AddServiceDecoration(loggerFactory, qi, qi.CreateGenericType(decorator));
                    else
                    {
                        // Not a generic decorator, so lets find the handler bits and fill them in.
                        var decoratorQis = decorator.GetCommandHandlerList();

                        foreach (var decoratorQi in decoratorQis)
                            services.AddServiceDecoration(loggerFactory, qi, decoratorQi.CreateGenericType(decorator));
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
        ///     <seealso cref="IQueryHandler{TRequestQuery, TResult}"/>, and each will be wrapped
        ///     with each decorator, this also holds true with the decorator itself.
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

            var type = typeof(THandler);
            var qis = type.GetQueryHandlerList();

            foreach (var decorator in decorators.Reverse())
            {
                foreach (var qi in qis)
                {
                    if (decorator.IsGenericType)
                        services.AddServiceDecoration(loggerFactory, qi, qi.CreateGenericType(decorator));
                    else
                    {
                        // Not a generic decorator, so lets find the handler bits and fill them in.
                        var decoratorQis = decorator.GetQueryHandlerList();

                        foreach (var decoratorQi in decoratorQis)
                            services.AddServiceDecoration(loggerFactory, qi, decoratorQi.CreateGenericType(decorator));
                    }
                }
            }

            return services;
        }

        private static Type CreateGenericType(this Type decorated, Type decorator)
        {
            var p = decorated.GetGenericArguments();
            var decParams = decorator.MakeGenericType(p);

            return decParams;
        }

        private static List<Type> GetQueryHandlerList(this Type type)
        {
            var ret = type.GetInterfaces()
                .Where(t =>
                    t.IsAssignableTo(typeof(IQueryHandler<>))
                    || t.IsAssignableTo(typeof(IQueryHandler<,>))
                ).ToList();

            if (!ret.Any())
                throw new ArgumentException(
                    $"The {type} must implement IQueryHandler<> or IQueryHandler<,>."
                );

            return ret;
        }

        private static List<Type> GetCommandHandlerList(this Type type)
        {
            var ret = type.GetInterfaces()
                .Where(t =>
                    t.IsAssignableTo(typeof(ICommandHandler<>))
                    || t.IsAssignableTo(typeof(ICommandHandler<,>))
                ).ToList();

            if (!ret.Any())
                throw new ArgumentException(
                    $"The {type} must implement ICommandHandler<> or ICommandHandler<,>."
                );

            return ret;
        }
    }
}
