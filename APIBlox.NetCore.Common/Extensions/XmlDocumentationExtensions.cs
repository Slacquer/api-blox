using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace APIBlox.NetCore.Extensions
{
    /// <summary>
    ///     Utility class to provide documentation for various types where available with the assembly
    /// </summary>
    public static class XmlDocumentationExtensions
    {
        /// <summary>
        ///     Paths to look into when default implementation fails to find files.
        /// </summary>
        public static List<string> FallbackPaths { get; set; }

        /// <summary>
        ///     A cache used to remember Xml documentation for assemblies.
        /// </summary>
        private static readonly Dictionary<Assembly, XmlDocument> Cache = new Dictionary<Assembly, XmlDocument>();

        /// <summary>
        ///     A cache used to store failure exceptions for assembly lookups.
        /// </summary>
        private static readonly Dictionary<Assembly, Exception> FailCache = new Dictionary<Assembly, Exception>();

        /// <summary>
        ///     Provides the documentation comments for a specific method.
        /// </summary>
        /// <param name="methodInfo">The MethodInfo (reflection data ) of the member to find documentation for</param>
        /// <returns>The XML fragment describing the method</returns>
        public static XmlElement GetDocumentation(this MethodInfo methodInfo)
        {
            // Calculate the parameter string as this is in the member name in the XML
            var parametersString = "";

            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                if (parametersString.Length > 0)
                    parametersString += ",";

                parametersString += parameterInfo.ParameterType.FullName;
            }

            //AL: 15.04.2008 ==> BUG-FIX remove “()” if parametersString is empty
            if (parametersString.Length > 0)
                return XmlFromName(methodInfo.DeclaringType, 'M', methodInfo.Name + "(" + parametersString + ")");

            return XmlFromName(methodInfo.DeclaringType, 'M', methodInfo.Name);
        }

        /// <summary>
        ///     Provides the documentation comments for a specific member.
        /// </summary>
        /// <param name="memberInfo">The MemberInfo (reflection data) or the member to find documentation for</param>
        /// <returns>The XML fragment describing the member</returns>
        public static XmlElement GetDocumentation(this MemberInfo memberInfo)
        {
            // First character [0] of member type is prefix character in the name in the XML
            return XmlFromName(memberInfo.DeclaringType, memberInfo.MemberType.ToString()[0], memberInfo.Name);
        }

        /// <summary>
        ///     Returns the Xml documentation summary comment for this member.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static string GetSummary(this MemberInfo memberInfo)
        {
            var element = memberInfo.GetDocumentation();
            var summaryElm = element?.SelectSingleNode("summary");

            return summaryElm == null ? "" : summaryElm.InnerText.Trim();
        }

        /// <summary>
        ///     Returns the Xml documentation remarks comment for this member.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static string GetRemarks(this MemberInfo memberInfo)
        {
            var element = memberInfo.GetDocumentation();
            var remarksElm = element?.SelectSingleNode("remarks");

            return remarksElm == null ? "" : remarksElm.InnerText.Trim();
        }

        /// <summary>
        ///     Provides the documentation comments for a specific type.
        /// </summary>
        /// <param name="type">Type to find the documentation for</param>
        /// <returns>The XML fragment that describes the type</returns>
        public static XmlElement GetDocumentation(this Type type)
        {
            // Prefix in type names is T
            return XmlFromName(type, 'T', "");
        }

        /// <summary>
        ///     Gets the summary portion of a type's documentation or returns an empty string if not available.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSummary(this Type type)
        {
            var element = type.GetDocumentation();
            var summaryElm = element?.SelectSingleNode("summary");

            return summaryElm == null ? "" : summaryElm.InnerText.Trim();
        }

        /// <summary>
        ///     Gets the remarks portion of a type's documentation or returns an empty string if not available.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetRemarks(this Type type)
        {
            var element = type.GetDocumentation();
            var remarksElm = element?.SelectSingleNode("remarks");

            return remarksElm == null ? "" : remarksElm.InnerText.Trim();
        }

        /// <summary>
        ///     Obtains the documentation file for the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to find the XML document for</param>
        /// <returns>The XML document</returns>
        /// <remarks>
        ///     This version uses a cache to preserve the assemblies, so that
        ///     the XML file is not loaded and parsed on every single lookup
        /// </remarks>
        public static XmlDocument XmlFromAssembly(this Assembly assembly)
        {
            if (FailCache.ContainsKey(assembly))
                throw FailCache[assembly];

            try
            {
                if (!Cache.ContainsKey(assembly))
                    Cache[assembly] = XmlFromAssemblyNonCached(assembly);

                return Cache[assembly];
            }
            catch (Exception exception)
            {
                FailCache[assembly] = exception;
                throw;
            }
        }

        /// <summary>
        ///     Obtains the XML Element that describes a reflection element by searching the
        ///     members for a member that has a name that describes the element.
        /// </summary>
        /// <param name="type">The type or parent type, used to fetch the assembly</param>
        /// <param name="prefix">The prefix as seen in the name attribute in the documentation XML</param>
        /// <param name="name">Where relevant, the full name qualifier for the element</param>
        /// <returns>The member that has a name that describes the specified reflection element</returns>
        private static XmlElement XmlFromName(this Type type, char prefix, string name)
        {
            string fullName;

            if (string.IsNullOrEmpty(name))
                fullName = prefix + ":" + type.FullName;
            else
                fullName = prefix + ":" + type.FullName + "." + name;

            var xmlDocument = XmlFromAssembly(type.Assembly);

            var matchedElement = xmlDocument["doc"]["members"].SelectSingleNode("member[@name='" + fullName + "']") as XmlElement;

            return matchedElement;
        }

        /// <summary>
        ///     Loads and parses the documentation file for the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to find the XML document for</param>
        /// <returns>The XML document</returns>
        private static XmlDocument XmlFromAssemblyNonCached(Assembly assembly)
        {
            const string prefix = "file:///";

            var assemblyFilename = assembly.CodeBase;
            var doc = new XmlDocument();
            var name = assembly.GetName().Name;
            var path = Path.ChangeExtension(assemblyFilename.Substring(prefix.Length), ".xml");

            try
            {
                var streamReader = new StreamReader(path);

                doc.Load(streamReader);
                return doc;
            }
            catch (FileNotFoundException exception)
            {
                if (!(FallbackPaths is null) && FallbackPaths.Count > 0)
                {
                    var searchFiles = new List<string>();
                    
                    foreach (var fPath in FallbackPaths)
                    {
                        var assPath = Path.Combine(fPath, $"{name}.xml");

                        if (File.Exists(assPath))
                        {
                            doc.Load(new StreamReader(assPath));

                            return doc;
                        }

                        searchFiles.Add(assPath);
                    }
                    throw new Exception(
                        $"XML documentation not present (make sure it is turned on in " +
                        $"project properties when building) for type {name}.  " +
                        $"Looked for: '{string.Join(",", searchFiles)}'.",
                        exception
                    );
                }

                throw new Exception(
                    $"XML documentation not present (make sure it is turned on in " +
                    $"project properties when building) for for type {name}.  Looked for xml file '{path}'.",
                    exception
                );
            }
        }
    }
}
