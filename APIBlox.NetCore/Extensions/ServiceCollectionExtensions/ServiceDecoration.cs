using System;
using System.Linq;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Extensions;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ServiceCollectionExtensionsNetCoreServiceDecoration.
    /// </summary>
    public static class ServiceCollectionExtensionsNetCoreServiceDecoration
    {
        private static ILogger _log;

        /// <summary>
        ///     Decorates ALL registered service(s) that match your decorator.
        /// </summary>
        /// <typeparam name="TService">The type of the t service.</typeparam>
        /// <typeparam name="TDecorator">The type of the t decorator.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="loggerFactory">ILoggerFactory</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddServiceDecoration<TService, TDecorator>(
            this IServiceCollection services,
            ILoggerFactory loggerFactory
        )
        {
            CreateLog(loggerFactory);

            return services.AddServiceDecoration(loggerFactory, typeof(TService), typeof(TDecorator));
        }

        /// <summary>
        ///     Decorates ALL registered service(s) that match your decorator.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="loggerFactory">ILoggerFactory</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="decoratorType">Type of the decorator.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddServiceDecoration(
            this IServiceCollection services,
             ILoggerFactory loggerFactory,
            Type serviceType, Type decoratorType
        )
        {
            CreateLog(loggerFactory);

            if (serviceType.IsOpenGeneric() && decoratorType.IsOpenGeneric())
                DecorateOpenGeneric(services, serviceType, decoratorType);
            else
                DecorateDescriptors(services, serviceType, x => DecorateService(x, decoratorType));

            return services;
        }

        private static void DecorateDescriptors(
            IServiceCollection services, Type serviceType,
            Func<ServiceDescriptor, ServiceDescriptor> decorator
        )
        {
            if (!TryDecorateDescriptors(services, serviceType, decorator))
                _log.LogCritical(() =>
                    $"Could not find any registered services for type {serviceType}. " +
                    " You need to add the service manually or decorate it " +
                    $"with the {nameof(InjectableServiceAttribute)}"
                );
        }

        private static void DecorateOpenGeneric(IServiceCollection services, Type serviceType, Type decoratorType)
        {
            var arguments = services
                .Where(descriptor => descriptor.ServiceType.IsAssignableTo(serviceType))
                .Select(descriptor => descriptor.ServiceType.GenericTypeArguments)
                .ToList();

            var valid = arguments.Count != 0 && arguments
                            .Aggregate(true,
                                (result, args) => result
                                                  && TryDecorate(services, args, serviceType, decoratorType)
                            );

            if (!valid)
            {
                var t = serviceType;
                _log.LogCritical(() =>
                    $"Could not find any registered services for type {t}."
                );
            }
        }

        private static ServiceDescriptor DecorateService(this ServiceDescriptor descriptor, Type decoratorType)
        {
            return ServiceDescriptor.Describe(descriptor.ServiceType,
                provider => ActivatorUtilities.CreateInstance(provider,
                    decoratorType,
                    provider.GetInstance(descriptor)
                ),
                descriptor.Lifetime
            );
        }

        private static object GetInstance(this IServiceProvider provider, ServiceDescriptor descriptor)
        {
            if (!(descriptor.ImplementationInstance is null))
                return descriptor.ImplementationInstance;

            return !(descriptor.ImplementationType is null)
                ? ActivatorUtilities.GetServiceOrCreateInstance(provider,
                    descriptor.ImplementationType
                )
                : descriptor.ImplementationFactory(provider);
        }

        private static bool TryDecorate(
            IServiceCollection services, Type[] typeArguments,
            Type serviceType, Type decoratorType
        )
        {
            var closedServiceType = serviceType.MakeGenericType(typeArguments);
            var closedDecoratorType = decoratorType.MakeGenericType(typeArguments);

            return TryDecorateDescriptors(services, closedServiceType, x => DecorateService(x, closedDecoratorType));
        }

        private static bool TryDecorateDescriptors(
            IServiceCollection services, Type serviceType,
            Func<ServiceDescriptor, ServiceDescriptor> decorator
        )
        {
            var descriptors = services.Where(service => service.ServiceType == serviceType).ToList();

            if (!descriptors.Any())
                return false;

            foreach (var descriptor in descriptors)
            {
                var index = services.IndexOf(descriptor);
                services.Insert(index, decorator(descriptor));
                services.Remove(descriptor);
            }

            return true;
        }

        private static void CreateLog(ILoggerFactory loggerFactory)
        {
            if (!(_log is null))
                return;

            _log = loggerFactory.CreateLogger("APIBlox.NetCore-ServiceDecoration");
        }
    }
}
