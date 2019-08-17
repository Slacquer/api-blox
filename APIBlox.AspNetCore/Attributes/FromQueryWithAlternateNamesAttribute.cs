using System;
using APIBlox.AspNetCore.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace APIBlox.AspNetCore.Attributes
{
    /// <summary>
    ///     Class FromQueryWithAlternateNamesAttribute.  Allows the <seealso cref="FromQueryAlternateNamesBinder" />
    ///     to bind different names.
    ///     Implements the <see cref="Microsoft.AspNetCore.Mvc.FromQueryAttribute" />
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class FromQueryWithAlternateNamesAttribute : FromQueryAttribute
    {
        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="FromQueryWithAlternateNamesAttribute" /> class.
        /// </summary>
        /// <param name="alternateNames">
        ///     The alternate names.  The first one is used as the default in
        ///     <see cref="FromQueryAttribute" />
        /// </param>
        public FromQueryWithAlternateNamesAttribute(string[] alternateNames)
        {
            Name = !(alternateNames is null) 
                ? alternateNames[0] 
                : throw new ArgumentNullException(nameof(alternateNames), "You must specify at least one name.");

            AlternateNames = alternateNames;
        }

        #endregion

        /// <summary>
        ///     Gets the alternate names.
        /// </summary>
        /// <value>The alternate names.</value>
        public string[] AlternateNames { get;}
    }
}
