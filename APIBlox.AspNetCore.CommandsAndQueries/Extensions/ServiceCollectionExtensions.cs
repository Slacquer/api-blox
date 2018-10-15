using System;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;

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
        public static IServiceCollection AddCommandHandlerDecoration<THandler>(
            this IServiceCollection services,
            params Type[] decorators
        )
        {
            if (!decorators.Any())
                return services;

            var type = typeof(THandler);

            var qi = type.GetInterfaces()
                .FirstOrDefault(t =>
                    t.IsAssignableTo(typeof(ICommandHandler<>))
                    || t.IsAssignableTo(typeof(ICommandHandler<,>))
                );

            if (qi is null)
                throw new ArgumentException($"The {type} must be an generic type of IQueryHandler<>, " +
                                            "IQueryHandler<,>, ICommandHandler<> or ICommandHandler<,>) "
                );

            foreach (var decorator in decorators.Reverse())
            {
                if (decorator.IsGenericType)
                {
                    var p = qi.GetGenericArguments();
                    var decParams = decorator.MakeGenericType(p);
                    services.AddServiceDecoration(qi, decParams);
                }
                else
                {
                    services.AddServiceDecoration(qi, decorator);
                }
            }

            return services;
        }

        /// <summary>
        ///     Decorates a REGISTERED query handler service.  Note that
        ///     decorations are executed in the order they are added.
        /// </summary>
        public static IServiceCollection AddQueryHandlerDecoration<THandler>(
            this IServiceCollection services,
            params Type[] decorators
        )
        {
            if (!decorators.Any())
                throw new ArgumentNullException(nameof(decorators), "Why would you decorate a handler with nothing?");

            var ths = typeof(THandler);

            var qi = ths.GetInterfaces()
                .FirstOrDefault(t =>
                    t.IsAssignableTo(typeof(IQueryHandler<,>))
                    || t.IsAssignableTo(typeof(IQueryHandler<>))
                );

            if (qi is null)
                throw new ArgumentException($"The {ths} must be an generic type of " +
                                            "IQueryHandler<>, IQueryHandler<,>, ICommandHandler<> or ICommandHandler<,>) "
                );

            foreach (var decorator in decorators.Reverse())
            {
                if (decorator.IsGenericType)
                {
                    var p = qi.GetGenericArguments();
                    var decParams = decorator.MakeGenericType(p);
                    services.AddServiceDecoration(qi, decParams);
                }
                else
                {
                    services.AddServiceDecoration(qi, decorator);
                }
            }

            return services;
        }
    }
}
