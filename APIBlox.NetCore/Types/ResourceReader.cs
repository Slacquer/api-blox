using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using APIBlox.NetCore.Extensions;

namespace APIBlox.NetCore.Types
{
    /// <summary>
    ///     Class EmbeddedResourceReader.
    /// </summary>
    /// <typeparam name="TCaller">The type of the t caller.</typeparam>
    public class EmbeddedResourceReader<TCaller>
    {
        private static readonly Assembly Ass = typeof(TCaller).GetTypeInfo().Assembly;

        /// <summary>
        ///     Gets an embedded resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>System.String.</returns>
        public static string GetResource(string resource)
        {
            var names = Ass.GetManifestResourceNames();
            var name = names.FirstOrDefault(r => r.EndsWithEx(resource));

            var resourceStream = name is null ? null : Ass.GetManifestResourceStream(name);

            if (resourceStream is null)
                throw new NullReferenceException(
                    $"Resource Not found, Found: {string.Join(", ", names)}"
                );

            using var reader = new StreamReader(resourceStream, Encoding.UTF8);

            return reader.ReadToEnd();
        }

        /// <summary>
        ///     Gets embedded resources.
        /// </summary>
        /// <param name="templatePath">The template path.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        /// <exception cref="NullReferenceException">
        ///     Resources for path '{templatePath}' Not found, Found: {string.Join(", ",
        ///     names)}
        /// </exception>
        public static Dictionary<string, string> GetResources(string templatePath)
        {
            var ret = new Dictionary<string, string>();

            var tp = $".{templatePath}.";
            var names = Ass.GetManifestResourceNames().Where(s => s.ContainsEx(tp)).ToList();

            if (!names.Any())
                throw new NullReferenceException(
                    $"Resources for path '{templatePath}' Not found, Found: {string.Join(", ", names)}"
                );

            foreach (var name in names)
            {
                var rs = Ass.GetManifestResourceStream(name);

                using var reader = new StreamReader(rs ?? throw new InvalidOperationException(), Encoding.UTF8);

                var str = name.Replace(".txt", "");
                var key = str[(str.LastIndexOfEx(".") + 1)..];

                ret.Add(key, reader.ReadToEnd());
            }

            return ret;
        }
    }
}
