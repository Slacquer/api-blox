using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using APIBlox.NetCore.Extensions;

namespace APIBlox.NetCore.Types
{
    /// <summary>
    ///     Class PathParser.
    /// </summary>
    public class PathParser
    {
        private readonly string[] _pathParts;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PathParser" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public PathParser(string path)
        {
            _pathParts = path.Split(new[] { "**" }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        ///     Gets a list of directory info objects using ** pattern matching.
        /// </summary>
        /// <param name="filterAction">Filter results while being added.</param>
        /// <returns>IEnumerable&lt;DirectoryInfo&gt;.</returns>
        public IEnumerable<DirectoryInfo> GetDirectories(Func<string, bool> filterAction = null)
        {
            if (_pathParts.Length == 1)
                return new[] { new DirectoryInfo(_pathParts[0]) };

            var paths = new List<string>();

            var root = new DirectoryInfo(_pathParts[0]).FullName;
            paths.AddRange(Directory.GetDirectories(root, "*", SearchOption.AllDirectories)
                .Where(d =>
                    {
                        var ret = _pathParts.Any(p => d.EndsWithEx(p))
                            && !paths.Any(di => di.EqualsEx(d));
                        
                        return ret && (filterAction?.Invoke(d) ?? true);
                    }
                )
            );

            return paths.Distinct().Select(p => new DirectoryInfo(p));
        }
    }
}
