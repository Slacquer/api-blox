using System;
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

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
