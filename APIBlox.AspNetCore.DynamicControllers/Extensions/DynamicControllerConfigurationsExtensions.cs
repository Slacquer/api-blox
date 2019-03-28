using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore;
using APIBlox.AspNetCore.Contracts;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    // TODO: Refactor.

    /// <summary>
    ///     Class DynamicControllerConfigurationsExtensions.
    /// </summary>
    public static class DynamicControllerConfigurationsExtensions
    {
        private static IInternalDynamicControllerConfigurationsService _configs;

        /// <summary>
        ///     Add controller/resource configuration that has no specific response type.
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="configurations">The configurations instance.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations.</returns>
        public static IDynamicControllerConfiguration AddController<TRequest>(
            this IDynamicControllerConfigurations configurations, string[] routes,
            params Type[] controllers
        )
        {
            return AddController<TRequest>(configurations, routes, null, controllers);
        }

        /// <summary>
        ///     Add controller/resource configuration that has no specific response type.
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="configurations">The configurations instance.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllerName">Controller name, if null this is generated from the last segment of the first route.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations.</returns>
        public static IDynamicControllerConfiguration AddController<TRequest>(
            this IDynamicControllerConfigurations configurations, string[] routes, string controllerName,
            params Type[] controllers
        )
        {
            if (!controllers.Any())
                throw new ArgumentNullException(
                    nameof(controllers),
                    "You must specify some kind of controller!"
                );

            if (_configs is null)
                _configs = (IInternalDynamicControllerConfigurationsService) configurations;

            var req = typeof(TRequest);
            var config = new DynamicControllerConfiguration(controllerName, req, null, routes);

            AddController(controllers, config, req);

            return config;
        }

        /// <summary>
        ///     Add controller/resource configuration that uses {TId} and both {TRequest} and {TResponse} (IE: POST)
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the t response.
        ///     <para>
        ///         This is the actual type, meaning you may be thinking to put
        ///         something like IEnumerable{MyType}, don't just use MyType.
        ///     </para>
        /// </typeparam>
        /// <typeparam name="TId">The type of the t id</typeparam>
        /// <param name="configurations">The configurations instance.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations.</returns>
        public static IDynamicControllerConfiguration AddController<TRequest, TResponse, TId>(this IDynamicControllerConfigurations configurations,
            string[] routes, params Type[] controllers
        )
            where TResponse : IResource<TId>
        {
            return AddController<TRequest, TResponse, TId>(configurations, routes, null, controllers);
        }

        /// <summary>
        ///     Add controller/resource configuration that uses {TId} and both {TRequest} and {TResponse} (IE: POST)
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the t response.
        ///     <para>
        ///         This is the actual type, meaning you may be thinking to put
        ///         something like IEnumerable{MyType}, don't just use MyType.
        ///     </para>
        /// </typeparam>
        /// <typeparam name="TId">The type of the t id</typeparam>
        /// <param name="configurations">The configurations instance.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllerName">Controller name, if null this is generated from the last segment of the first route.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations.</returns>
        public static IDynamicControllerConfiguration AddController<TRequest, TResponse, TId>(
            this IDynamicControllerConfigurations configurations, string[] routes, string controllerName,
            params Type[] controllers
        )
            where TResponse : IResource<TId>
        {
            if (!controllers.Any())
                throw new ArgumentNullException(
                    nameof(controllers),
                    "You must specify some kind of controller!"
                );

            if (_configs is null)
                _configs = (IInternalDynamicControllerConfigurationsService) configurations;

            var req = typeof(TRequest);
            var res = typeof(TResponse);
            var id = typeof(TId);
            var config = new DynamicControllerConfiguration(controllerName, req, null, routes);

            AddController3(controllers, config, req, res, id);

            return config;
        }
        
        /// <summary>
        ///     Add controller/resource configuration that uses both {TRequest} and {TResponse} (IE: POST)
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the t response.
        ///     <para>
        ///         This is the actual type, meaning you may be thinking to put
        ///         something like IEnumerable{MyType}, don't just use MyType.
        ///     </para>
        /// </typeparam>
        /// <param name="configurations">The configurations instance.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations.</returns>
        public static IDynamicControllerConfiguration AddController<TRequest, TResponse>(this IDynamicControllerConfigurations configurations,
            string[] routes, params Type[] controllers
        )
            where TResponse : IResource
        {
            return AddController<TRequest, TResponse>(configurations, routes, null, controllers);
        }

        /// <summary>
        ///     Add controller/resource configuration that uses both {TRequest} and {TResponse} (IE: POST)
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the t response.
        ///     <para>
        ///         This is the actual type, meaning you may be thinking to put
        ///         something like IEnumerable{MyType}, don't just use MyType.
        ///     </para>
        /// </typeparam>
        /// <param name="configurations">The configurations instance.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllerName">Controller name, if null this is generated from the last segment of the first route.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations.</returns>
        public static IDynamicControllerConfiguration AddController<TRequest, TResponse>(
            this IDynamicControllerConfigurations configurations, string[] routes, string controllerName,
            params Type[] controllers
        )
            where TResponse : IResource
        {
            if (!controllers.Any())
                throw new ArgumentNullException(
                    nameof(controllers),
                    "You must specify some kind of controller!"
                );

            if (_configs is null)
                _configs = (IInternalDynamicControllerConfigurationsService) configurations;

            var req = typeof(TRequest);
            var res = typeof(TResponse);
            var config = new DynamicControllerConfiguration(controllerName, req, null, routes);

            AddController2(controllers, config, req, res);

            return config;
        }
        

        /// <summary>
        ///     Add sub controller/resource configuration that has no specific response type.
        ///     <para>Remember this returns the PARENT Controller!</para>
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="parentConfiguration">The parent configuration.</param>
        /// <param name="parentIdType">Type of the parent controllers ID.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations, the parent controller.</returns>
        public static IDynamicControllerConfiguration AddSubController<TRequest>(
            this IDynamicControllerConfiguration parentConfiguration, Type parentIdType, string[] routes,
            params Type[] controllers
        )
        {
            return AddSubController<TRequest>(parentConfiguration, parentIdType, routes, null, controllers);
        }

        /// <summary>
        ///     Add sub controller/resource configuration that uses {TId} and both {TRequest} and {TResponse}
        ///     <para>Remember this returns the PARENT Controller!</para>
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <param name="parentConfiguration">The parent configuration.</param>
        /// <param name="parentIdType">Type of the parent controllers ID.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllerName">Controller name, if null this is generated from the last segment of the first route.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations, the parent controller.</returns>
        public static IDynamicControllerConfiguration AddSubController<TRequest>(
            this IDynamicControllerConfiguration parentConfiguration, Type parentIdType, string[] routes,
            string controllerName,
            params Type[] controllers
        )
        {
            if (!controllers.Any())
                throw new ArgumentNullException(
                    nameof(controllers),
                    "You must specify some kind of controller!"
                );

            var pc = (DynamicControllerConfiguration) parentConfiguration;
            var req = typeof(TRequest);

            var config = new DynamicControllerConfiguration(controllerName,
                req,
                parentIdType,
                BuildFromParentRoute(parentConfiguration, routes)
            )
            {
                ParentController = pc
            };

            AddController(controllers, config, req);

            return config;
        }

        /// <summary>
        ///     Add sub controller/resource configuration that uses {TId} and both {TRequest} and {TResponse} (IE: POST)
        ///     <para>Remember this returns the PARENT Controller!</para>
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the t response.
        ///     <para>
        ///         This is the actual type, meaning you may be thinking to put
        ///         something like IEnumerable{MyType}, don't just use MyType.
        ///     </para>
        /// </typeparam>
        /// <typeparam name="TId">The type of the t id</typeparam>
        /// <param name="parentConfiguration">The parent configuration.</param>
        /// <param name="parentIdType">Type of the parent controllers ID.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations, the parent controller.</returns>
        public static IDynamicControllerConfiguration AddSubController<TRequest, TResponse, TId>(
            this IDynamicControllerConfiguration parentConfiguration, Type parentIdType, string[] routes,
            params Type[] controllers
        )
            where TResponse : IResource<TId>
        {
            return AddSubController<TRequest, TResponse, TId>(parentConfiguration,
                parentIdType,
                routes,
                null,
                controllers
            );
        }

        /// <summary>
        ///     Add sub controller/resource configuration that uses {TId} and both {TRequest} and {TResponse} (IE: POST)
        ///     <para>Remember this returns the PARENT Controller!</para>
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the t response.
        ///     <para>
        ///         This is the actual type, meaning you may be thinking to put
        ///         something like IEnumerable{MyType}, don't just use MyType.
        ///     </para>
        /// </typeparam>
        /// <typeparam name="TId">The type of the t id</typeparam>
        /// <param name="parentConfiguration">The parent configuration.</param>
        /// <param name="parentIdType">Type of the parent controllers ID.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllerName">Controller name, if null this is generated from the last segment of the first route.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations, the parent controller.</returns>
        public static IDynamicControllerConfiguration AddSubController<TRequest, TResponse, TId>(
            this IDynamicControllerConfiguration parentConfiguration, Type parentIdType, string[] routes,
            string controllerName,
            params Type[] controllers
        )
            where TResponse : IResource<TId>
        {
            if (!controllers.Any())
                throw new ArgumentNullException(
                    nameof(controllers),
                    "You must specify some kind of controller!"
                );

            var pc = (DynamicControllerConfiguration) parentConfiguration;
            var req = typeof(TRequest);
            var res = typeof(TResponse);
            var id = typeof(TId);

            var config = new DynamicControllerConfiguration(controllerName,
                req,
                parentIdType,
                BuildFromParentRoute(parentConfiguration, routes)
            )
            {
                ParentController = pc
            };

            AddController3(controllers, config, req, res, id);

            return config;
        }
        
        /// <summary>
        ///     Add sub controller/resource configuration that uses both {TRequest} and {TResponse} (IE: POST)
        ///     <para>Remember this returns the PARENT Controller!</para>
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the t response.
        ///     <para>
        ///         This is the actual type, meaning you may be thinking to put
        ///         something like IEnumerable{MyType}, don't just use MyType.
        ///     </para>
        /// </typeparam>
        /// <param name="parentConfiguration">The parent configuration.</param>
        /// <param name="parentIdType">Type of the parent controllers ID.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations, the parent controller.</returns>
        public static IDynamicControllerConfiguration AddSubController<TRequest, TResponse>(
            this IDynamicControllerConfiguration parentConfiguration, Type parentIdType, string[] routes,
            params Type[] controllers
        )
            where TResponse : IResource
        {
            return AddSubController<TRequest, TResponse>(parentConfiguration,
                parentIdType,
                routes,
                null,
                controllers
            );
        }

        /// <summary>
        ///     Add sub controller/resource configuration that uses both {TRequest} and {TResponse} (IE: POST)
        ///     <para>Remember this returns the PARENT Controller!</para>
        ///     <para>
        ///         WARNING!  When your routes are the same yet the route params are different, be sure to use route constraints!
        ///         IE: foo/{id}/bar is no different than foo/{key}/bar, unless the id for example is {id:int}.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRequest">The type of the t request.</typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the t response.
        ///     <para>
        ///         This is the actual type, meaning you may be thinking to put
        ///         something like IEnumerable{MyType}, don't just use MyType.
        ///     </para>
        /// </typeparam>
        /// <param name="parentConfiguration">The parent configuration.</param>
        /// <param name="parentIdType">Type of the parent controllers ID.</param>
        /// <param name="routes">The routes.</param>
        /// <param name="controllerName">Controller name, if null this is generated from the last segment of the first route.</param>
        /// <param name="controllers">The controllers.</param>
        /// <returns>IDynamicControllerConfigurations, the parent controller.</returns>
        public static IDynamicControllerConfiguration AddSubController<TRequest, TResponse>(
            this IDynamicControllerConfiguration parentConfiguration, Type parentIdType, string[] routes,
            string controllerName,
            params Type[] controllers
        )
            where TResponse : IResource
        {
            if (!controllers.Any())
                throw new ArgumentNullException(
                    nameof(controllers),
                    "You must specify some kind of controller!"
                );

            var pc = (DynamicControllerConfiguration) parentConfiguration;
            var req = typeof(TRequest);
            var res = typeof(TResponse);

            var config = new DynamicControllerConfiguration(controllerName,
                req,
                parentIdType,
                BuildFromParentRoute(parentConfiguration, routes)
            )
            {
                ParentController = pc
            };

            AddController2(controllers, config, req, res);

            return config;
        }

        private static void AddController(
            IEnumerable<Type> controllers,
            DynamicControllerConfiguration config, Type request
        )
        {
            foreach (var controller in controllers)
            {
                ParamCheck(controller, 1);
                config.ControllerType = controller.MakeGenericType(request);
                _configs.AddControllerConfig(config);
            }
        }

        private static void AddController2(
            IEnumerable<Type> controllers,
            DynamicControllerConfiguration config, Type request, Type response
        )
        {
            foreach (var controller in controllers)
            {
                ParamCheck(controller, 2);
                config.ControllerType = controller.MakeGenericType(request, response);
                _configs.AddControllerConfig(config);
            }
        }

        private static void AddController3(
            IEnumerable<Type> controllers,
            DynamicControllerConfiguration config, Type request, Type response, Type id
        )
        {
            foreach (var controller in controllers)
            {
                ParamCheck(controller, 3);
                config.ControllerType = controller.MakeGenericType(request, response, id);
                _configs.AddControllerConfig(config);
            }
        }

        private static string[] BuildFromParentRoute(
            IDynamicControllerConfiguration parentConfiguration,
            string[] routes
        )
        {
            var ret = new List<string>();
            var parentRoutes = ((DynamicControllerConfiguration) parentConfiguration).Routes;

            foreach (var parentRoute in parentRoutes)
                ret.AddRange(routes.Select(s => $"{parentRoute}/{s}".Replace("//", "/")));

            return ret.ToArray();
        }

        private static void ParamCheck(Type controller, int maxParams)
        {
            var count = controller.GetGenericArguments().Count();

            if (count == maxParams)
                return;

            throw new ArgumentException(
                $"The controller {controller.Name} takes {count} params, you specified " +
                $"{maxParams}, your using the wrong method, try the other one."
            );
        }
    }
}
