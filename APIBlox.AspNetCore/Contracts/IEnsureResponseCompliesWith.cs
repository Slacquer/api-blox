#region -    Using Statements    -

using System;

#endregion

namespace APIBlox.AspNetCore.Contracts
{
    /// <summary>
    ///     Interface IEnsureResponseCompliesWith
    /// </summary>
    public interface IEnsureResponseCompliesWith
    {
        /// <summary>
        ///     Gets or sets the function.
        /// </summary>
        /// <value>The function.</value>
        Func<object, object> Func { get; set; }
    }
}
