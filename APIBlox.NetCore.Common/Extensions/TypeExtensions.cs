using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace APIBlox.NetCore.Extensions
{
    /// <summary>
    ///     Class TypeExtensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class TypeExtensions
    {
        // Cached results.
        private static readonly List<MethodInfo> Extensions = new();

        /// <summary>
        ///     Sets the value of a nullable type.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <param name="obj">The object instance.</param>
        /// <param name="qp">The qp.</param>
        public static void SetNullableValue(this PropertyInfo prop, object obj, object qp)
        {
            var t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            var safeValue = qp == null ? null : Convert.ChangeType(qp, t);

            prop.SetValue(obj, safeValue, null);
        }

        /// <summary>
        ///     Find an extension method for a given type with a specific
        ///     name (case insensitive).  Optionally filter the result.
        /// </summary>
        public static MethodInfo GetExtensionMethod(
            this Type type,
            string methodName, Func<MethodInfo, bool> filterFunc
        )
        {
            return type.GetExtensionMethods(methodName).Where(filterFunc).FirstOrDefault();
        }

        /// <summary>
        ///     Find extension methods for a given type.  Optionally
        ///     with a specific name (case insensitive).
        ///     <para>
        ///         Results are cached in local list.
        ///     </para>
        /// </summary>
        public static IEnumerable<MethodInfo> GetExtensionMethods(
            this Type type,
            string methodName = null
        )
        {
            var found = Extensions.Where(mi =>
                mi.GetParameters().Any(pi =>
                    pi.ParameterType == type && mi.Name.EqualsEx(methodName ?? mi.Name)
                )
            ).ToList();

            if (found.Any())
                return found;

            var assTypes = new List<Type>();

            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
                assTypes.AddRange(item.GetTypes());

            var mn = methodName;
            var objType = type;

            var lst = assTypes.Where(t => t.IsSealed && !t.IsGenericType && !t.IsNested)
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic),
                    (t, method) => new
                    {
                        mn,
                        objType,
                        method
                    }
                )
                .Where(t =>
                    t.method.Name.EqualsEx(mn ?? t.method.Name)
                    && t.method.IsDefined(typeof(ExtensionAttribute), false)
                    && t.method.GetParameters()[0].ParameterType == objType
                )
                .Select(t => t.method);

            Extensions.AddRange(lst.Except(Extensions));

            return Extensions.ToList();
        }

        /// <summary>
        ///     Determines whether a type is assignable to another.  This has always been a PITA and
        ///     for years have gotten this backwards, so this method will check, generic, non-generic
        ///     where t1.IsAssignableFrom(t2) OR t2.IsAssignableFrom(t1).  Yes I'm that kind of stupid.
        /// </summary>
        /// <param name="type">The type, generic or not.</param>
        /// <param name="otherType">Type of the other, generic or not.</param>
        /// <returns><c>true</c> if can be assigned; otherwise, <c>false</c>.</returns>
        public static bool IsAssignableTo(this Type type, Type otherType)
        {
            var typeInfo = type.GetTypeInfo();
            var otherTypeInfo = otherType.GetTypeInfo();

            if (!otherTypeInfo.IsGenericTypeDefinition)
                return typeInfo.IsAssignableFrom(otherTypeInfo) || otherTypeInfo.IsAssignableFrom(typeInfo);

            return typeInfo.IsGenericTypeDefinition
                ? typeInfo.Equals(otherTypeInfo)
                : typeInfo.IsAssignableToGenericTypeDefinition(otherTypeInfo);
        }

        /// <summary>
        ///     Determines whether a type is an open generic.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is open generic] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsOpenGeneric(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition;
        }

        /// <summary>
        ///     Gets all json property name values for a type that is using the <see cref="JsonPropertyAttribute" />.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Dictionary&lt;PropertyInfo, System.String&gt;.</returns>
        public static Dictionary<PropertyInfo, string> JsonPropertyNames(this Type type)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var lst = new Dictionary<PropertyInfo, string>();

            foreach (var pi in props)
            {
                if (pi.GetCustomAttributes(typeof(JsonPropertyAttribute), false).FirstOrDefault() is not JsonPropertyAttribute att)
                    continue;

                lst.Add(pi, att.PropertyName);
            }

            return lst;
        }

        private static bool IsAssignableToGenericTypeDefinition(
            this Type type,
            Type genericTypeInfo
        )
        {
            var interfaceTypes = type.GetTypeInfo().ImplementedInterfaces.Select(t => t.GetTypeInfo());

            if (interfaceTypes.Where(interfaceType => interfaceType.IsGenericType)
                .Select(interfaceType => interfaceType.GetGenericTypeDefinition().GetTypeInfo())
                .Any(typeDefinitionTypeInfo => typeDefinitionTypeInfo == genericTypeInfo))
                return true;

            if (type.IsGenericType)
            {
                var typeDefinitionTypeInfo = type
                    .GetGenericTypeDefinition()
                    .GetTypeInfo();

                if (typeDefinitionTypeInfo == genericTypeInfo)
                    return true;
            }

            var baseTypeInfo = type.BaseType?.GetTypeInfo();

            return baseTypeInfo?.IsAssignableToGenericTypeDefinition(genericTypeInfo) == true;
        }
    }
}
