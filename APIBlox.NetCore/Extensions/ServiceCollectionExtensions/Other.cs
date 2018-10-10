﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using APIBlox.NetCore;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class APIBloxNetCoreServiceCollectionExtensions.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        private static ILogger _log;

        private static readonly List<KeyValuePair<bool, Type>> AssemblyTypes = new List<KeyValuePair<bool, Type>>();

        /// <summary>
        ///     Adds a service that requires Dependency Injection, while performing a setup action.
        /// </summary>
        /// <typeparam name="TOptions">The type of the t options.</typeparam>
        /// <typeparam name="TDependent">The type of the t dependent.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="setupAction">The setup action.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// <returns>IServiceCollection.</returns>
        /// <autogeneratedoc />
        public static IServiceCollection AddDependencyWithOptions<TOptions, TDependent>(
            this IServiceCollection services, Action<TOptions, TDependent> setupAction,
            ServiceLifetime lifetime = ServiceLifetime.Singleton
        )
            where TOptions : class
            where TDependent : class
        {
            services.Add(new ServiceDescriptor(typeof(TDependent), typeof(TDependent), lifetime));

            services.AddSingleton<IConfigureOptions<TOptions>, ConfigureOptionsWithDependencyContainer<TOptions, TDependent>>();
            services.Configure<ConfigureOptionsWithDependency<TOptions, TDependent>>(options => options.Action = setupAction);

            return services;
        }

        /// <summary>
        ///     Adds injectable services by finding all classes decorated with
        ///     <see cref="InjectableServiceAttribute" /> in the given namespace(s) and path(s).
        ///     <para>
        ///         This only applies to NON-NESTED interfaces.
        ///     </para>
        ///     <para>
        ///         Assembly paths for Executing, Calling and Entry are always searched.
        ///     </para>
        ///     <para>
        ///         Be sure to add me as close to the beginning of the service collection chain as possible.
        ///     </para>
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="loggerFactory">ILoggerFactory</param>
        /// <param name="assemblyNamesLike">The assembly names like.</param>
        /// <param name="assemblyPaths">The assembly paths.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddInjectableServices(
            this IServiceCollection services,
            ILoggerFactory loggerFactory,
            string[] assemblyNamesLike, string[] assemblyPaths = null
        )
        {
            CreateLog(loggerFactory);

            InitializeAssemblyTypes(true, false, assemblyNamesLike, assemblyPaths);
            var injectable = AssemblyTypes.Where(kvp => !kvp.Key).ToList();

            if (!injectable.Any())
                throw new ArgumentException($"No types decorated with {nameof(InjectableServiceAttribute)} could be found using" +
                                            $" provided pattern(s) for {nameof(assemblyNamesLike)} and {nameof(assemblyPaths)}.  " +
                                            "If this is intentional, please remove the " +
                                            $"{nameof(AddInjectableServices)} entry."
                );

            foreach (var kvp in injectable)
                services.RegisterServiceType(kvp.Value);

            return services;
        }

        /// <summary>
        ///     Adds Startup like configurations that implement <see cref="IDependencyInvertedConfiguration" />
        ///     and calls <see cref="IDependencyInvertedConfiguration.Configure" /> for
        ///     those found in the given namespace(s) and path(s).
        ///     <para>
        ///         Assembly paths for Executing, Calling and Entry are always searched.
        ///     </para>
        ///     <para>
        ///         Be sure to add me as close to the beginning of the service collection chain as possible.
        ///     </para>
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="loggerFactory">ILoggerFactory</param>
        /// <param name="environment">
        ///     Name of running environment, we are not Passing IHostingEnvironment
        ///     as this is a AspNetCore thing, my understanding is that eventually the GenericHostBuilder will likely
        ///     expose the environment, and we can use it rather than a string.
        /// </param>
        /// <param name="configuration">IConfiguration</param>
        /// <param name="assemblyNamesLike">The assembly names like.</param>
        /// <param name="assemblyPaths">The assembly paths.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddInvertedDependentsAndConfigureServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            string environment,
            string[] assemblyNamesLike, string[] assemblyPaths = null
        )
        {
            CreateLog(loggerFactory);

            InitializeAssemblyTypes(false, true, assemblyNamesLike, assemblyPaths);
            var inverted = AssemblyTypes.Where(kvp => kvp.Key).ToList();

            if (!inverted.Any())
                throw new ArgumentException("No types that implement " +
                                            $"{nameof(IDependencyInvertedConfiguration)} could be found using" +
                                            $" provided pattern(s) for {nameof(assemblyNamesLike)} and {nameof(assemblyPaths)}.  " +
                                            "If this is intentional, please remove the " +
                                            $"{nameof(AddInvertedDependentsAndConfigureServices)} entry."
                );

            foreach (var kvp in inverted)
                ConfigureInverted(kvp.Value, services, configuration, loggerFactory, environment);

            return services;
        }

        private static ServiceDescriptor BuildDescriptor(
            Type type,
            Type instance, ServiceLifetime lifetime
        )
        {
            return new ServiceDescriptor(type, instance, lifetime);
        }

        private static void ConfigureInverted(
            Type type,
            IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory, string environment
        )
        {
            if (type.GetConstructors().All(c => c.GetParameters().Length != 0))
                throw new ArgumentException($"Implementations of {nameof(IDependencyInvertedConfiguration)} " +
                                            "must have a parameter-less constructor."
                );

            ((IDependencyInvertedConfiguration) Activator.CreateInstance(type))
                .Configure(services, configuration, loggerFactory, environment);
        }

        private static IEnumerable<string> GetAssemblyFilePaths(
            string[] assemblyNamesLike,
            IEnumerable<string> assemblyPaths
        )
        {
            var asses = new List<string>();

            foreach (var path in assemblyPaths)
            {
                var di = new DirectoryInfo(path);
                var fullPath = di.FullName;

                if (!di.Exists)
                    throw new DirectoryNotFoundException($"Path {fullPath} not found, please make sure your configuration is correct.");

                asses.AddRange(Directory.GetFiles(fullPath, "*.dll", SearchOption.AllDirectories)
                    .Where(s => assemblyNamesLike.Any(name => s.ContainsEx(name))
                                && asses.Select(Path.GetFileName).All(fn =>
                                    !fn.EqualsEx(Path.GetFileName(s))
                                )
                    )
                );
            }

            return asses;
        }

        private static IEnumerable<KeyValuePair<bool, Type>> GetResolvedTypes(
            bool injectable, bool inverted,
            IEnumerable<string> asses
        )
        {
            var ret = new List<KeyValuePair<bool, Type>>();

            foreach (var ass in asses)
            {
                try
                {
                    using (var assResolver = new AssemblyResolver(ass))
                    {
                        ret.AddRange(assResolver.Assembly.GetTypes()
                            .Where(x =>
                                !x.GetTypeInfo().IsAbstract && injectable &&
                                x.GetCustomAttributes<InjectableServiceAttribute>().Any()
                                || inverted && x.GetInterfaces().Any(t => typeof(IDependencyInvertedConfiguration).IsAssignableTo(t))
                            )
                            .Select(t => new KeyValuePair<bool, Type>(typeof(IDependencyInvertedConfiguration).IsAssignableTo(t), t))
                        );
                    }
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is BadImageFormatException)
                {
                    _log.LogWarning(() => ex.Message);
                }
            }

            return ret;
        }

        private static void InitializeAssemblyTypes(
            bool injectable,
            bool inverted,
            string[] assemblyNamesLike,
            IEnumerable<string> assemblyPaths = null
        )
        {
            if (!assemblyNamesLike.Any())
                throw new ArgumentException("You must specify at least one assembly name pattern.");

            var callingLocation = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            var entryLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var def = new[] {callingLocation, entryLocation, executingLocation};
            var aps = assemblyPaths?.Union(def) ?? def;

            var found = GetResolvedTypes(injectable,
                inverted,
                GetAssemblyFilePaths(assemblyNamesLike, aps)
            ).Except(AssemblyTypes).ToList();

            _log.LogInformation(() =>
                string.Format(injectable
                        ? "\n{1} list:\n{0}"
                        : "\n{2} list:\n{0}",
                    string.Join(",\n", found.Select(k => k.Value).ToList()),
                    nameof(InjectableServiceAttribute),
                    nameof(IDependencyInvertedConfiguration)
                )
            );

            AssemblyTypes.AddRange(found);
        }

        private static void RegisterServiceType(this IServiceCollection services, Type type)
        {
            var interfaces = type.GetInterfaces();
            var attr = type.GetCustomAttribute<InjectableServiceAttribute>();
            var lifetime = attr.ServiceLifetime;

            if (!interfaces.Any())
                throw new ArgumentException($"Services decorated with the {nameof(InjectableServiceAttribute)} " +
                                            "must implement at least 1 interface.",
                    nameof(type)
                );

            foreach (var face in interfaces.Where(i => !i.IsNested))
            {
                var args = face.GetGenericArguments();
                ServiceDescriptor descriptor;

                if (args?.Any() == true && type.IsGenericTypeDefinition)
                {
                    if (type.IsOpenGeneric())
                    {
                        var tt = face.Assembly.GetTypes().First(t => t.Name == face.Name);
                        descriptor = BuildDescriptor(tt, type, lifetime);
                    }
                    else
                    {
                        if (args.Any())
                        {
                            var decParams = type.MakeGenericType(args);
                            descriptor = BuildDescriptor(decParams, type, lifetime);
                        }
                        else
                        {
                            descriptor = BuildDescriptor(face, type, lifetime);
                        }
                    }
                }

                // If interface doesn't have any generic args, then we will have nothing to pass
                // to the implementation during creation for THIS INTERFACE, so we will not add it as a service.
                else if (args == null || !args.Any() && type.IsGenericTypeDefinition)
                {
                    continue;
                }
                else
                {
                    descriptor = BuildDescriptor(face, type, lifetime);
                }

                services.Add(descriptor);
            }
        }

        private static void CreateLog(ILoggerFactory loggerFactory)
        {
            if (!(_log is null))
                return;

            _log = loggerFactory.CreateLogger("APIBlox.NetCore.ServiceCollectionExtensions.Other");
        }
    }
}
