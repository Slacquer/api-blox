using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using APIBlox.NetCore.Extensions;
using Microsoft.Extensions.DependencyModel;

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
        private Dictionary<string, string> _assPathCache = new Dictionary<string, string>();
        private bool _disposed;
        private Dictionary<string, Assembly> _loadedAssemblyCache = new Dictionary<string, Assembly>();

        /// <summary>
        ///     Loads an assembly and all its referenced assemblies.
        /// </summary>
        /// <param name="assemblyFileInfo">The assembly file information.</param>
        /// <param name="alreadyLoaded">True when assembly has already been loaded</param>
        /// <returns>Assembly.</returns>
        public Assembly LoadFromAssemblyFileInfo(FileInfo assemblyFileInfo, out bool alreadyLoaded)
        {
            return LoadFromAssemblyPath(assemblyFileInfo.FullName, out alreadyLoaded);
        }

        /// <summary>
        ///     Loads an assembly and all its referenced assemblies.
        /// </summary>
        /// <param name="assemblyFullPath">The assembly full path.</param>
        /// <param name="alreadyLoaded">True when assembly has already been loaded</param>
        /// <returns>Assembly.</returns>
        public Assembly LoadFromAssemblyPath(string assemblyFullPath, out bool alreadyLoaded)
        {
            if (assemblyFullPath.IsEmptyNullOrWhiteSpace() || !File.Exists(assemblyFullPath))
                throw new ArgumentException(
                    $"Invalid assembly file path: {assemblyFullPath}",
                    nameof(assemblyFullPath)
                );

            var fileName = Path.GetFileName(assemblyFullPath);

            var loaded = fileName;

            if (_loadedAssemblyCache.Keys.Contains(loaded))
            {
                alreadyLoaded = true;

                return _loadedAssemblyCache[loaded];
            }

            var fileNameWithOutExtension = Path.GetFileNameWithoutExtension(assemblyFullPath);
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
                LoadReferencedAssemblies(assembly, fileName, directory);

            alreadyLoaded = false;

            return assembly;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _disposed = true;

            _assPathCache.Clear();
            _loadedAssemblyCache.Clear();

            _assPathCache = null;
            _loadedAssemblyCache = null;
        }

        private void LoadReferencedAssemblies(Assembly assembly, string fileName, string directory)
        {
            var loaded = fileName;

            if (_loadedAssemblyCache.Keys.Contains(loaded))
                return;

            _loadedAssemblyCache.Add(loaded, assembly);

            var filesInDirectory = GetAssemblyFiles(directory).ToList();
            var references = assembly.GetReferencedAssemblies().OrderBy(a => a.Name);

            foreach (var reference in references)
            {
                try
                {
                    var lfn = $"{reference.Name}.dll";

                    if (_loadedAssemblyCache.Keys.Contains(lfn))
                        continue;

                    var path = Path.Combine(directory, lfn);

                    if (!filesInDirectory.Contains(path))
                        continue;

                    var loadedAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);

                    if (loadedAssembly == null)
                        continue;

                    LoadReferencedAssemblies(loadedAssembly, lfn, directory);
                }
                catch (FileLoadException)
                {
                    // Don't care just move on...
                }
            }
        }

        private IEnumerable<string> GetAssemblyFiles(string filePath)
        {
            var tmp = Directory.GetFiles(filePath, "*.dll")
                .Where(s => !_assPathCache.ContainsKey(Path.GetFileName(s)))
                .Select(s => new KeyValuePair<string, string>(Path.GetFileName(s), s));

            foreach (var kvp in tmp)
                _assPathCache.Add(kvp.Key, kvp.Value);

            return _assPathCache.Select(kvp => kvp.Value).OrderBy(s => s);
        }
    }
}
