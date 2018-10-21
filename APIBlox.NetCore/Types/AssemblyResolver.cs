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
    /// <summary>
    ///     Class AssemblyResolver. This class cannot be inherited.
    ///     <para>Use me to load assemblies that are not referenced, very helpful for the D in SOLID.</para>
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    public sealed class AssemblyResolver
    {
        private readonly Dictionary<string, string> _assCache = new Dictionary<string, string>();
        private readonly List<string> _loadedCache = new List<string>();

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
                LoadReferencedAssemblies(assembly, fileName, directory);

            return assembly;
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
