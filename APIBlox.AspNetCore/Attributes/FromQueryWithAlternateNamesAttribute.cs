using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.ModelBinders;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace APIBlox.AspNetCore.Attributes
{
    /// <summary>
    ///     Class FromQueryWithAlternateNamesAttribute.  Allows the <seealso cref="FromQueryAlternateNamesBinder" />
    ///     to bind different names.
    ///     Implements the <see cref="Microsoft.AspNetCore.Mvc.FromQueryAttribute" />
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class FromQueryWithAlternateNamesAttribute : Attribute, IBindingSourceMetadata, IModelNameProvider
    {
        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="FromQueryWithAlternateNamesAttribute" /> class.
        /// </summary>
        /// <param name="alternateNames">
        ///     The alternate names.  The first one is used as the default in
        ///     <see cref="FromQueryAttribute" />
        /// </param>
        public FromQueryWithAlternateNamesAttribute(params string[] alternateNames)
        {
            Name = !(alternateNames is null) && alternateNames.Length > 0
                ? alternateNames[0]
                : throw new ArgumentNullException(nameof(alternateNames),
                    "You must at least specify the default name!  However if you are not using " +
                    $"alternates then you should consider using the {nameof(FromQueryAttribute)} instead."
                );

            foreach (var an in alternateNames)
            {
                if (AlternateNames.Any(s => s.EqualsEx(an)))
                    continue;

                AlternateNames.Add(an);
            }
        }

        #endregion

        /// <summary>
        ///     Gets the alternate names.
        /// </summary>
        /// <value>The alternate names.</value>
        public HashSet<string> AlternateNames { get; } = new HashSet<string>();

        /// <summary>
        ///     Gets the <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.IBindingSourceMetadata.BindingSource" />.
        /// </summary>
        /// <value>The binding source.</value>
        /// <remarks>The <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.IBindingSourceMetadata.BindingSource" /> is metadata which can be used to determine which data
        /// sources are valid for model binding of a property or parameter.</remarks>
        public BindingSource BindingSource => BindingSource.Query;

        /// <summary>
        ///     Model name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
    }
}
