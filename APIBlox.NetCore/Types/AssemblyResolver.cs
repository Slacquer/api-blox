using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using APIBlox.NetCore.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace APIBlox.NetCore.Types
{
    /// <inheritdoc />
    /// <summary>
    ///     Class AssemblyResolver. This class cannot be inherited.
    ///     <para>Use me to load assemblies that are not referenced, very helpful for the D in SOLID.</para>
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    public sealed class AssemblyResolver : IDisposable
    {
        private readonly ICompilationAssemblyResolver _assemblyResolver;
        private readonly DependencyContext _dependencyContext;
        private readonly AssemblyLoadContext _loadContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AssemblyResolver" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public AssemblyResolver(string path)
        {
            var p = new DirectoryInfo(path).FullName;
            Assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(p);
            _dependencyContext = DependencyContext.Load(Assembly);

            _assemblyResolver = new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
                {
                    new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(p)),
                    new ReferenceAssemblyPathResolver(),
                    new PackageCompilationAssemblyResolver()
                }
            );
            _loadContext = AssemblyLoadContext.GetLoadContext(Assembly);
            _loadContext.Resolving += OnResolving;
        }

        /// <summary>
        ///     Gets the assembly.
        /// </summary>
        /// <value>The assembly.</value>
        public Assembly Assembly { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _loadContext.Resolving -= OnResolving;
        }

        private Assembly OnResolving(AssemblyLoadContext context, AssemblyName name)
        {
            var library = _dependencyContext.RuntimeLibraries
                .FirstOrDefault(r => r.Name.EqualsEx(name.Name));

            if (library is null)
                return null;

            var assemblies = new List<string>();

            var wrapper = new CompilationLibrary(library.Type,
                library.Name,
                library.Version,
                library.Hash,
                library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                library.Dependencies,
                library.Serviceable
            );
            _assemblyResolver.TryResolveAssemblyPaths(wrapper, assemblies);

            return assemblies.Count > 0
                ? _loadContext.LoadFromAssemblyPath(assemblies[0])
                : null;
        }
    }
}
