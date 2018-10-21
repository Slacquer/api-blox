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
    /// <summary>
    ///     Class AssemblyResolver. This class cannot be inherited.
    ///     <para>Use me to load assemblies that are not referenced, very helpful for the D in SOLID.</para>
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    public sealed class AssemblyResolver
    {
        private readonly Dictionary<string, string> _assCache = new Dictionary<string, string>();
        private readonly List<string> _loadedCache = new List<string>();

        private ICompilationAssemblyResolver _assemblyResolver;
        private AssemblyLoadContext _loadContext;
        private DependencyContext _dependencyContext;

        /// <summary>
        ///     Loads an assembly and all its referenced assemblies.
        /// </summary>
        /// <param name="assemblyFileInfo">The assembly file information.</param>
        /// <returns>Assembly.</returns>
        public Assembly LoadFromAssemblyFileInfo(FileInfo assemblyFileInfo)
        {
            return LoadFromAssemblyPath(assemblyFileInfo.FullName);
        }

        /// <summary>
        ///     Loads an assembly and all its referenced assemblies.
        /// </summary>
        /// <param name="assemblyFullPath">The assembly full path.</param>
        /// <returns>Assembly.</returns>
        public Assembly LoadFromAssemblyPath(string assemblyFullPath)
        {
            if (assemblyFullPath.IsEmptyNullOrWhiteSpace() || !File.Exists(assemblyFullPath))
                throw new ArgumentException(
                    $"Invalid assembly file path: {assemblyFullPath}",
                    nameof(assemblyFullPath)
                );

            if (_loadedCache.Contains(assemblyFullPath))
                return null;

            var fileNameWithOutExtension = Path.GetFileNameWithoutExtension(assemblyFullPath);
            var fileName = Path.GetFileName(assemblyFullPath);
            var directory = Path.GetDirectoryName(assemblyFullPath);

            var inCompileLibraries = DependencyContext.Default.CompileLibraries.Any(l =>
                l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase)
            );
            var inRuntimeLibraries = DependencyContext.Default.RuntimeLibraries.Any(l =>
                l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase)
            );

            var assembly = inCompileLibraries || inRuntimeLibraries
                ? Assembly.Load(new AssemblyName(fileNameWithOutExtension))
                : AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFullPath);

            if (assembly != null)
            {
                _dependencyContext = DependencyContext.Load(assembly);

                _assemblyResolver = new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
                    {
                        new AppBaseCompilationAssemblyResolver(directory),
                        new ReferenceAssemblyPathResolver(),
                        new PackageCompilationAssemblyResolver()
                    }
                );

                _loadContext = AssemblyLoadContext.GetLoadContext(assembly);
                
                _loadContext.Resolving += OnResolving;

                LoadReferencedAssemblies(assembly, fileName, directory);
            }

            return assembly;
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

        private void LoadReferencedAssemblies(Assembly assembly, string fileName, string directory)
        {
            var loaded = Path.Combine(directory, fileName);

            if (_loadedCache.Contains(loaded))
                return;

            _loadedCache.Add(loaded);

            var filesInDirectory = GetAssemblyFiles(directory).ToList();
            var references = assembly.GetReferencedAssemblies().OrderBy(a => a.Name);

            foreach (var reference in references)
            {
                var lfn = $"{reference.Name}.dll";
                var path = Path.Combine(directory, lfn);

                if (!filesInDirectory.Contains(path))
                    continue;

                var loadedAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);

                if (loadedAssembly == null)
                    continue;

                LoadReferencedAssemblies(loadedAssembly, lfn, directory);
            }
        }

        private IEnumerable<string> GetAssemblyFiles(string filePath)
        {
            var tmp = Directory.GetFiles(filePath, "*.dll")
                .Where(s => !_assCache.ContainsKey(Path.GetFileName(s)))
                .Select(s => new KeyValuePair<string, string>(Path.GetFileName(s), s));

            foreach (var kvp in tmp)
                _assCache.Add(kvp.Key, kvp.Value);

            return _assCache.Select(kvp => kvp.Value).OrderBy(s => s);
        }
    }
}








//#region -    Using Statements    -

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.Loader;
//using APIBlox.NetCore.Extensions;
//using Microsoft.Extensions.DependencyModel;
//using Microsoft.Extensions.DependencyModel.Resolution;

//#endregion

//namespace APIBlox.NetCore
//{
//    /// <inheritdoc />
//    /// <summary>
//    ///     Class AssemblyResolver. This class cannot be inherited.
//    ///     <para>Use me to load assemblies that are not referenced, very helpful for the D in SOLID.</para>
//    /// </summary>
//    /// <seealso cref="T:System.IDisposable" />
//    public sealed class AssemblyResolver : IDisposable
//    {
//        #region -    Fields    -

//        private readonly ICompilationAssemblyResolver _assemblyResolver;
//        private readonly DependencyContext _dependencyContext;
//        private readonly AssemblyLoadContext _loadContext;

//        #endregion

//        #region -    Constructors    -

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="AssemblyResolver" /> class.
//        /// </summary>
//        /// <param name="path">The path.</param>
//        public AssemblyResolver(string path)
//        {
//            var p = new DirectoryInfo(path).FullName;
//            Assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(p);
//            _dependencyContext = DependencyContext.Load(Assembly);

//            _assemblyResolver = new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
//                {
//                    new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(p)),
//                    new ReferenceAssemblyPathResolver(),
//                    new PackageCompilationAssemblyResolver()
//                }
//            );
//            _loadContext = AssemblyLoadContext.GetLoadContext(Assembly);
//            _loadContext.Resolving += OnResolving;
//        }

//        #endregion

//        /// <summary>
//        ///     Gets the assembly.
//        /// </summary>
//        /// <value>The assembly.</value>
//        public Assembly Assembly { get; }

//        /// <inheritdoc />
//        /// <summary>
//        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
//        /// </summary>
//        public void Dispose()
//        {
//            _loadContext.Resolving -= OnResolving;
//        }

//        private Assembly OnResolving(AssemblyLoadContext context, AssemblyName name)
//        {
//            var library = _dependencyContext.RuntimeLibraries
//                .FirstOrDefault(r => r.Name.EqualsEx(name.Name));

//            if (library is null)
//                return null;

//            var assemblies = new List<string>();

//            var wrapper = new CompilationLibrary(library.Type,
//                library.Name,
//                library.Version,
//                library.Hash,
//                library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
//                library.Dependencies,
//                library.Serviceable
//            );
//            _assemblyResolver.TryResolveAssemblyPaths(wrapper, assemblies);

//            return assemblies.Count > 0
//                ? _loadContext.LoadFromAssemblyPath(assemblies[0])
//                : null;
//        }
//    }
//}
