﻿#region -    Using Statements    -

using System;

#endregion

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Options
{
    /// <summary>
    ///     Class ConfigureOptionsWithDependency.
    /// </summary>
    /// <typeparam name="TOptions">The type of the t options.</typeparam>
    /// <typeparam name="TDependent">The type of the t dependent.</typeparam>
    /// <autogeneratedoc />
    public class ConfigureOptionsWithDependency<TOptions, TDependent>
    {
        /// <summary>
        ///     Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        /// <autogeneratedoc />
        public Action<TOptions, TDependent> Action { get; set; }
    }
}
