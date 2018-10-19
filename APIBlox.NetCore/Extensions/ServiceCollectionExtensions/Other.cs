﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class APIBloxNetCoreServiceCollectionExtensions.
    /// </summary>
    public static partial class ServiceCollectionExtensionsNetCore
    {
        private static ILogger _log;
        private static List<string> _excludedPaths = new List<string>();
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
        ///         Be sure to add me as close to the beginning of the service collection chain as possible.
        ///     </para>
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="loggerFactory">ILoggerFactory</param>
        /// <param name="assemblyNamesLike">The assembly names like.</param>
        /// <param name="assemblyPaths">The assembly paths, supporting absolute, relative and ** or ! to exclude.</param>
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
            {
                _log.LogError(() =>
                    $"No types decorated with {nameof(InjectableServiceAttribute)} could be found using" +
                    $" provided pattern(s) for {nameof(assemblyNamesLike)} and {nameof(assemblyPaths)}.  " +
                    "If this is intentional, please remove the " +
                    $"{nameof(AddInjectableServices)} entry."
                );
                return services;
            }

            foreach (var kvp in injectable)
                services.RegisterServiceType(kvp.Value);

            return services;
        }

        /// <summary>
        ///     Adds Startup like configurations that implement <see cref="IDependencyInvertedConfiguration" />
        ///     and calls <see cref="IDependencyInvertedConfiguration.Configure" /> for
        ///     those found in the given namespace(s) and path(s).
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
        /// <param name="assemblyPaths">The assembly paths, supporting absolute, relative and ** or ! to exclude.</param>
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
            {
                _log.LogError(() =>
                    "No types that implement " +
                    $"{nameof(IDependencyInvertedConfiguration)} could be found using" +
                    $" provided pattern(s) for {nameof(assemblyNamesLike)} and {nameof(assemblyPaths)}.  " +
                    "If this is intentional, please remove the " +
                    $"{nameof(AddInvertedDependentsAndConfigureServices)} entry."
                );
                return services;
            }

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
            {
                _log.LogError(() =>
                    $"Implementations of {nameof(IDependencyInvertedConfiguration)} " +
                    "must have a parameter-less constructor."
                );
                return;
            }

            ((IDependencyInvertedConfiguration)Activator.CreateInstance(type))
                .Configure(services, configuration, loggerFactory, environment);
        }

        private static void InitializeAssemblyTypes(
            bool injectable,
            bool inverted,
            string[] assemblyNamesLike,
            IReadOnlyCollection<string> assemblyPaths = null
        )
        {
            if (assemblyNamesLike is null || !assemblyNamesLike.Any())
            {
                _log.LogError(() => "You must specify at least one assembly name pattern.");
                return;
            }

            if (assemblyPaths is null || !assemblyPaths.Any())
            {
                _log.LogError(() => "You must specify at least one assembly path.");
                return;
            }

            var found = GetResolvedTypes(injectable,
                inverted,
                GetAssemblyFiles(assemblyNamesLike, assemblyPaths)
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

        private static IEnumerable<string> GetAssemblyFiles(
            string[] assemblyNamesLike,
            IEnumerable<string> assemblyPaths
        )
        {
            var lst = assemblyPaths.ToList();
            var excluded = lst.Where(s => s.StartsWith("!")).ToList();
            var included = lst.Except(excluded);

            _excludedPaths.AddRange(excluded
                .SelectMany(s => PathParser.FindAll(s.Replace("!", ""))
                    .Select(di => di.FullName)
                )
                .Except(_excludedPaths)
            );

            List<string> actualPaths = null;

            foreach (var path in included)
            {
                actualPaths = PathParser.FindAll(path,
                        d => !_excludedPaths.Any(s => s.Contains(d) || d.Contains(s))
                    ).Select(d => d.FullName)
                    .ToList();
            }

            _log.LogInformation(() => string.Format("Excluded Search Paths: \n{0}",
                    string.Join(",\n", _excludedPaths.OrderBy(s => s))
                )
            );

            var ret = new List<string>();

            if (actualPaths is null || !actualPaths.Any())
                return ret;

            var ordered = actualPaths.OrderBy(s => s);

            _log.LogInformation(() => string.Format("Included Search Paths: \n{0}",
                    string.Join(",\n", ordered)
                )
            );

            foreach (var actualPath in ordered)
            {
                _log.LogInformation(() => $"Searching {actualPath} for assemblies.");

                ret.AddRange(Directory.GetFiles(actualPath, "*.dll")
                    .Where(s =>
                        assemblyNamesLike.Any(name =>
                            Path.GetFileName(s).ContainsEx(name)
                        )
                        && ret.Select(Path.GetFileName)
                            .All(fn =>
                                !fn.EqualsEx(Path.GetFileName(s))
                            )
                    )
                );
            }

            return ret;
        }

        private static IEnumerable<KeyValuePair<bool, Type>> GetResolvedTypes(
            bool injectable, bool inverted,
            IEnumerable<string> assemblyFiles
        )
        {
            var ret = new List<KeyValuePair<bool, Type>>();
            var assResolver = new AssemblyResolver();

            foreach (var ass in assemblyFiles)
            {
                try
                {
                    var path = Path.GetDirectoryName(ass);

                    if (_excludedPaths.Any(s => s.ContainsEx(path) || path.ContainsEx(s)))
                    {
                        _log.LogInformation(() => $"Skipping {ass}, it lives in one of the specified excluded paths.");
                        continue;
                    }

                    _log.LogInformation(() => $"Attempting to resolve: {ass}");

                    var assembly = assResolver.LoadFromAssemblyPath(ass);

                    if (assembly is null)
                        continue;

                    ret.AddRange(assembly.GetTypes()
                        .Where(x =>
                            !x.GetTypeInfo().IsAbstract && injectable &&
                            x.GetCustomAttributes<InjectableServiceAttribute>().Any()
                            || inverted && x.GetInterfaces().Any(t => typeof(IDependencyInvertedConfiguration).IsAssignableTo(t))
                        )
                        .Select(t => new KeyValuePair<bool, Type>(typeof(IDependencyInvertedConfiguration).IsAssignableTo(t), t))
                    );
                }
                catch (Exception ex) 
                    //when (
                    //ex is InvalidOperationException
                    //|| ex is BadImageFormatException
                    //|| ex is ReflectionTypeLoadException)
                {
                    _log.LogWarning(() => ex.Message);
                }
            }

            return ret;
        }

        private static void RegisterServiceType(this IServiceCollection services, Type type)
        {
            var interfaces = type.GetInterfaces();
            var attr = type.GetCustomAttribute<InjectableServiceAttribute>();
            var lifetime = attr.ServiceLifetime;

            if (!interfaces.Any())
            {
                _log.LogError(() =>
                    $"Services decorated with the {nameof(InjectableServiceAttribute)}  " +
                    "must implement at least 1 interface."
                );
                return;
            }

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
