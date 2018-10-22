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
    public static class PathParser
    {
        /// <summary>
        ///     Builds a list of directory info objects using ** pattern matching.
        /// </summary>
        /// <param name="searchPath">The search path.</param>
        /// <param name="filterAction">The filter action.</param>
        /// <returns>IEnumerable&lt;DirectoryInfo&gt;.</returns>
        /// <exception cref="NullReferenceException">Empty path!</exception>
        public static IEnumerable<DirectoryInfo> FindAllSubDirectories(string searchPath, Func<string, bool> filterAction = null)
        {
            if (searchPath.IsEmptyNullOrWhiteSpace())
                throw new NullReferenceException("Empty path!");

            var parts = searchPath.Split(new[] { "**" }, StringSplitOptions.RemoveEmptyEntries);

            var root = parts[0];
            var excludes = parts.Except(new[] { root }).Select(RemoveTrailingSlash).ToList();

            var ret = new List<DirectoryInfo>();
            var rootDi = new DirectoryInfo(root);

            if (!Directory.Exists(rootDi.FullName))
                return ret;

            ret.AddRange(Directory.GetDirectories(rootDi.FullName, "*", SearchOption.AllDirectories)
                .Where(s => excludes.All(e => s.ContainsEx(e))
                    && (filterAction?.Invoke(s) ?? true)
                )
                .Select(s => new DirectoryInfo(s))
            );

            return ret;
        }

        private static string RemoveTrailingSlash(string path)
        {
            return path.EndsWith(@"\") ? path.Substring(0, path.Length - 1) : path;
        }
    }
}
