#region -    Using Statements    -

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Filters;

#endregion

namespace APIBlox.AspNetCore.Extensions
{
    /// <summary>
    ///     Class CollectionExtensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class CollectionExtensions
    {
        /// <summary>
        ///     Adds a type to a <see cref="FilterCollection" /> if it doesn't already exist.
        /// </summary>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="col">The col.</param>
        /// <param name="value">The value.</param>
        /// <returns>FilterCollection.</returns>
        public static FilterCollection TryAdd<TType>(
            this FilterCollection col,
            TType value
        )
        {
            return col.TryAdd<FilterCollection, IFilterMetadata>((IFilterMetadata) value);
        }

        /// <summary>
        ///     Adds a type to a <see cref="FilterCollection" /> if it doesn't already exist.
        /// </summary>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="col">The col.</param>
        /// <returns>FilterCollection.</returns>
        public static FilterCollection TryAdd<TType>(this FilterCollection col)
            where TType : IFilterMetadata
        {
            var vt = typeof(TType);

            if (col.All(t =>
                {
                    // Yet more lameness, really should be able to do a simple test here...
                    switch (t)
                    {
                        case TypeFilterAttribute tf: return tf.ImplementationType != vt;
                        case ServiceFilterAttribute sf: return sf.ServiceType != vt;
                        case FormatFilterAttribute ff: return ff.GetType() != vt;
                        case ResultFilterAttribute rs: return rs.GetType() != vt;
                        case ExceptionFilterAttribute ef: return ef.GetType() != vt;
                        case ActionFilterAttribute af: return af.GetType() != vt;
                        default: return t.GetType() != vt;
                    }
                }
            ))
                col.Add(vt);

            return col;
        }

        /// <summary>
        ///     Adds a type to a <see cref="IList{IApplicationFeatureProvider}" /> if it doesn't already exist.
        /// </summary>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="col">The col.</param>
        /// <param name="value">The value.</param>
        /// <returns>IList{IApplicationFeatureProvider}.</returns>
        public static IList<IApplicationFeatureProvider> TryAdd<TType>(
            this IList<IApplicationFeatureProvider> col,
            TType value
        )
        {
            return col.TryAdd<IList<IApplicationFeatureProvider>,
                IApplicationFeatureProvider>((IApplicationFeatureProvider) value);
        }

        /// <summary>
        ///     Adds a type to a <see cref="IList{IApplicationModelConvention}" /> if it doesn't already exist.
        /// </summary>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="col">The col.</param>
        /// <param name="value">The value.</param>
        /// <returns>IList{IApplicationModelConvention}.</returns>
        public static IList<IApplicationModelConvention> TryAdd<TType>(
            this IList<IApplicationModelConvention> col,
            TType value
        )
            where TType : IApplicationModelConvention
        {
            return col.TryAdd<IList<IApplicationModelConvention>, IApplicationModelConvention>(value);
        }
    }
}
