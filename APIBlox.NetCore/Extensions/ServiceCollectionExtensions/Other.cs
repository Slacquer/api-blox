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
    public static class ServiceCollectionExtensionsNetCoreOther
    {
        private static ILogger _log;

        private static readonly List<KeyValuePair<bool, Type>>
            WorkingAssemblyTypes = new List<KeyValuePair<bool, Type>>();

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
        /// <param name="assemblyPaths">
        ///     The assembly paths, a prefix of '!' (without ticks) will indicate
        ///     a path NOT to use search.  If you need template  parsing <see cref="PathParser" />
        ///     <para>
        ///         Path searching includes sub directories
        ///     </para>
        /// </param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddInjectableServices(
            this IServiceCollection services, ILoggerFactory loggerFactory,
            string[] assemblyNamesLike, string[] assemblyPaths
        )
        {
            CreateLog(loggerFactory);

            InitializeAssemblyTypes(true, false, assemblyNamesLike, assemblyPaths);

            var injectable = WorkingAssemblyTypes.Where(kvp => !kvp.Key).ToList();

            if (!injectable.Any())
            {
                _log.LogError(() =>
                    $"No types decorated with {nameof(InjectableServiceAttribute)} could be found using" +
                    $" provided pattern(s) for {nameof(assemblyNamesLike)} and {nameof(assemblyPaths)}.  " +
                    "If this is intentional, please remove the " +
                    $"{nameof(AddInjectableServices)} entry.\n\n"
                );
                return services;
            }

            foreach (var kvp in injectable)
                services.RegisterServiceType(kvp.Value);

            return services;
        }

        /// <summary>
        ///     Adds injectable services by finding all classes decorated with
        ///     <see cref="InjectableServiceAttribute" /> that are referenced in the AppDomain.
        ///     <para>
        ///         This only applies to NON-NESTED interfaces.
        ///     </para>
        ///     <para>
        ///         Be sure to add me as close to the beginning of the service collection chain as possible.
        ///     </para>
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddReferencedInjectableServices(
            this IServiceCollection services, ILoggerFactory loggerFactory
        )
        {
            CreateLog(loggerFactory);

            FillWorkingAssemblyTypes(true, false);

            var injectable = WorkingAssemblyTypes.Where(kvp => !kvp.Key).ToList();

            if (!injectable.Any())
            {
                _log.LogError(() =>
                    $"No types decorated with {nameof(InjectableServiceAttribute)} could be found.  " +
                    "If this is intentional, please remove the " +
                    $"{nameof(AddReferencedInjectableServices)} entry.\n\n"
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
        ///     Name of running environment, we are not Passing IWebHostEnvironment
        ///     as this is a AspNetCore thing, my understanding is that eventually the GenericHostBuilder will likely
        ///     expose the environment, and we can use it rather than a string.
        /// </param>
        /// <param name="configuration">IConfiguration</param>
        /// <param name="assemblyNamesLike">The assembly names like.</param>
        /// <param name="assemblyPaths">
        ///     The assembly paths, a prefix of '!' (without ticks) will indicate
        ///     a path NOT to use search.  If you need template  parsing <see cref="PathParser" />
        ///     <para>
        ///         Path searching includes sub directories
        ///     </para>
        /// </param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddInvertedDependentsAndConfigureServices(
            this IServiceCollection services, IConfiguration configuration,
            ILoggerFactory loggerFactory, string environment,
            string[] assemblyNamesLike, string[] assemblyPaths
        )
        {
            CreateLog(loggerFactory);

            InitializeAssemblyTypes(false, true, assemblyNamesLike, assemblyPaths);

            var inverted = WorkingAssemblyTypes.Where(kvp => kvp.Key).ToList();

            if (!inverted.Any())
            {
                _log.LogError(() =>
                    "No types that implement " +
                    $"{nameof(IDependencyInvertedConfiguration)} could be found using" +
                    $" provided pattern(s) for {nameof(assemblyNamesLike)} and {nameof(assemblyPaths)}.  " +
                    "If this is intentional, please remove the " +
                    $"{nameof(AddInvertedDependentsAndConfigureServices)} entry.\n\n"
                );
                return services;
            }

            foreach (var kvp in inverted)
                ConfigureInverted(kvp.Value, services, configuration, loggerFactory, environment);

            return services;
        }

        /// <summary>
        ///     Adds Startup like configurations that implement <see cref="IDependencyInvertedConfiguration" />
        ///     and calls <see cref="IDependencyInvertedConfiguration.Configure" /> for classes
        ///     that are referenced in the AppDomain
        ///     <para>
        ///         Be sure to add me as close to the beginning of the service collection chain as possible.
        ///     </para>
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="loggerFactory">ILoggerFactory</param>
        /// <param name="environment">
        ///     Name of running environment, we are not Passing IWebHostEnvironment
        ///     as this is a AspNetCore thing, my understanding is that eventually the GenericHostBuilder will likely
        ///     expose the environment, and we can use it rather than a string.
        /// </param>
        /// <param name="configuration">IConfiguration</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddReferencedInvertedDependentsAndConfigureServices(
            this IServiceCollection services, IConfiguration configuration,
            ILoggerFactory loggerFactory, string environment
        )
        {
            CreateLog(loggerFactory);

            FillWorkingAssemblyTypes(false, true);

            var inverted = WorkingAssemblyTypes.Where(kvp => kvp.Key).ToList();

            if (!inverted.Any())
            {
                _log.LogError(() =>
                    "No types that implement " +
                    $"{nameof(IDependencyInvertedConfiguration)} could be found.  " +
                    "If this is intentional, please remove the " +
                    $"{nameof(AddReferencedInvertedDependentsAndConfigureServices)} entry.\n\n"
                );
                return services;
            }

            foreach (var kvp in inverted)
                ConfigureInverted(kvp.Value, services, configuration, loggerFactory, environment);

            return services;
        }

        private static void InitializeAssemblyTypes(
            bool injectable, bool inverted, string[] assemblyNamesLike,
            IReadOnlyCollection<string> assemblyPaths = null
        )
        {
            if (assemblyNamesLike is null || !assemblyNamesLike.Any())
                throw new NullReferenceException("You must specify at least one assembly name pattern.");

            if (assemblyPaths is null || !assemblyPaths.Any())
                throw new NullReferenceException("You must specify at least one assembly path.");

            var workingPaths = BuildDirectoriesList(assemblyPaths);
            var workingAssemblyFiles = GetWorkingAssemblyFileInfos(assemblyNamesLike, workingPaths).ToList();

            if (!workingAssemblyFiles.Any())
                throw new ArgumentException(
                    "Search results indicate there are NO assemblies to" +
                    " work within the given assemblyPaths!",
                    nameof(assemblyPaths)
                );

            var notAlreadyInWorkingAssemblyTypes = GetResolvedAssemblyTypes(
                injectable,
                inverted,
                workingAssemblyFiles
            ).Except(WorkingAssemblyTypes);

            _log.LogInformation(() =>
                string.Format(injectable
                        ? "\n{1} list:\n{0}"
                        : "\n{2} list:\n{0}",
                    string.Join(",\n", notAlreadyInWorkingAssemblyTypes.Select(k => k.Value).ToList()),
                    nameof(InjectableServiceAttribute),
                    nameof(IDependencyInvertedConfiguration)
                )
            );

            WorkingAssemblyTypes.AddRange(notAlreadyInWorkingAssemblyTypes);
        }

        private static IEnumerable<DirectoryInfo> BuildDirectoriesList(
            IReadOnlyCollection<string> assemblyPaths
        )
        {
            // if not absolute then log error and skip it.
            var included = assemblyPaths.Where(s => !s.TrimStart().StartsWith("!"));
            var excluded = assemblyPaths.Where(s => s.TrimStart().StartsWith("!"));

            var absIncluded = GetAbsDirectoryInfos(included, true);
            var absExcluded = GetAbsDirectoryInfos(excluded, false).ToList();

            return absExcluded.Any()
                ? absIncluded.Where(absDi =>
                    !absExcluded.Any(exDi => absDi.FullName.StartsWithEx(exDi.FullName))
                )
                : absIncluded;
        }

        private static IEnumerable<DirectoryInfo> GetAbsDirectoryInfos(
            IEnumerable<string> paths, bool including
        )
        {
            var absDis = new List<DirectoryInfo>();

            foreach (var p in paths)
            {
                var path = including ? p : p.StartsWith("!") ? p.Substring(1) : p;

                try
                {
                    var absPath = Path.GetFullPath(path);
                    var di = new DirectoryInfo(absPath);

                    if (!di.Exists)
                        throw new DirectoryNotFoundException();

                    absDis.Add(di);
                }
                catch (Exception ex)
                {
                    var be = including ? "INCLUDED" : "EXCLUDED";
                    _log.LogError(() => $"Error building DirectoryInfo from {path}.  Error: {ex.Message}.  It will be NOT be {be}!");
                }
            }

            return absDis;
        }

        private static IEnumerable<FileInfo> GetWorkingAssemblyFileInfos(
            string[] assemblyNamesLike,
            IEnumerable<DirectoryInfo> pathsToSearchOnly
        )
        {
            var ret = new List<FileInfo>();

            foreach (var di in pathsToSearchOnly)
            {
                ret.AddRange(di.GetFiles("*.dll")
                    .Where(s =>

                        // In include assembly names?
                        assemblyNamesLike.Any(name => s.Name.ContainsEx(name))

                        // Not already listed?
                        && ret.All(fi => !fi.FullName.EqualsEx(s.FullName))
                    )
                );
            }

            return ret;
        }

        private static IEnumerable<KeyValuePair<bool, Type>> GetResolvedAssemblyTypes(
            bool injectable, bool inverted,
            IEnumerable<FileInfo> assemblyFiles
        )
        {
            var ret = new List<KeyValuePair<bool, Type>>();

            foreach (var assFi in assemblyFiles)
            {
                try
                {
                    using (var assResolver = new AssemblyResolver())
                    {
                        if (!assFi.Exists)
                        {
                            _log.LogWarning(() => $"Skipping {assFi}, it no longer exists!");
                            continue;
                        }

                        var assembly = assResolver.LoadFromAssemblyFileInfo(assFi, out var alreadyLoaded);

                        if (assembly is null)
                        {
                            if (!alreadyLoaded)
                                _log.LogWarning(() => $"NULL result from LoadFromAssemblyFileInfo for file: {assFi}");

                            continue;
                        }

                        _log.LogInformation(() => $"Resolved Assembly: {assFi}");

                        ret.AddRange(assembly.GetTypes()
                            .Where(x =>
                                !x.GetTypeInfo().IsAbstract && injectable &&
                                x.GetCustomAttributes<InjectableServiceAttribute>().Any()
                                || inverted && x.GetInterfaces().Any(t =>
                                    typeof(IDependencyInvertedConfiguration).IsAssignableTo(t)
                                )
                            )
                            .Select(t => new KeyValuePair<bool, Type>(
                                    typeof(IDependencyInvertedConfiguration).IsAssignableTo(t),
                                    t
                                )
                            )
                        );
                    }
                }
                catch (Exception ex)
                {
                    _log.LogWarning(() => ex.Message);
                }
            }

            return ret;
        }

        private static void FillWorkingAssemblyTypes(bool injectable, bool inverted)
        {
            var tmp = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()
                .Where(x =>
                    !x.GetTypeInfo().IsAbstract && injectable &&
                    x.GetCustomAttributes<InjectableServiceAttribute>().Any()
                    || inverted && x.GetInterfaces().Any(t =>
                        typeof(IDependencyInvertedConfiguration).IsAssignableTo(t)
                    )
                )
                .Select(t => new KeyValuePair<bool, Type>(
                        typeof(IDependencyInvertedConfiguration).IsAssignableTo(t),
                        t
                    )
                )
            ).Except(WorkingAssemblyTypes);

            _log.LogInformation(() =>
                string.Format(injectable
                        ? "\nReferenced {1} list:\n{0}\n\n"
                        : "\nReferenced {2} list:\n{0}\n\n",
                    string.Join(",\n", tmp.Select(k => k.Value).ToList()),
                    nameof(InjectableServiceAttribute),
                    nameof(IDependencyInvertedConfiguration)
                )
            );

            WorkingAssemblyTypes.AddRange(tmp);
        }

        private static void RegisterServiceType(
            this IServiceCollection services, Type type
        )
        {
            var interfaces = type.GetInterfaces();
            var attr = type.GetCustomAttribute<InjectableServiceAttribute>();
            var lifetime = attr.ServiceLifetime;

            if (!interfaces.Any())
            {
                _log.LogError(() =>
                    $"Services decorated with the {nameof(InjectableServiceAttribute)}  " +
                    $"must implement at least 1 interface.  Therefore {type} will be skipped."
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

        private static ServiceDescriptor BuildDescriptor(
            Type type, Type instance, ServiceLifetime lifetime
        )
        {
            return new ServiceDescriptor(type, instance, lifetime);
        }

        private static void ConfigureInverted(
            Type type, IServiceCollection services, IConfiguration configuration,
            ILoggerFactory loggerFactory, string environment
        )
        {
            if (type.GetConstructors().All(c => c.GetParameters().Length != 0))
            {
                _log.LogError(() =>
                    $"Implementations of {nameof(IDependencyInvertedConfiguration)} " +
                    $"must have a parameter-less constructor.  Therefore {type} will be skipped."
                );
                return;
            }

            ((IDependencyInvertedConfiguration) Activator.CreateInstance(type))
                .Configure(services, configuration, loggerFactory, environment);
        }

        private static void CreateLog(ILoggerFactory loggerFactory)
        {
            if (!(_log is null))
                return;

            _log = loggerFactory.CreateLogger("APIBlox.NetCore-Other");
        }
    }
}
