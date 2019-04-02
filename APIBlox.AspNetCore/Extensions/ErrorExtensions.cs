using System;
using System.Diagnostics;
using APIBlox.AspNetCore.Types;

namespace APIBlox.AspNetCore.Extensions
{
    /// <summary>
    ///     Class ErrorExtensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class ErrorExtensions
    {
        internal static DynamicErrorObject ToDynamicDataObject(this Exception ex, bool noThrow)
        {
            var ret = new DynamicErrorObject("Error Details", ex.Message)
            {
                NoThrow = noThrow
            };

            dynamic d = ret;

            d.Type = ex.GetType().Name;

            if (ex.InnerException is null)
                return ret;

            ret.Title = "Please refer to the error property for additional information.";
            ret.Errors.Add(ex.InnerException.ToDynamicDataObject(noThrow));

            return ret;
        }

        /// <summary>
        ///     Builds an <see cref="DynamicErrorObject" /> from an exception along with all inner exceptions.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>DynamicErrorObject.</returns>
        public static DynamicErrorObject ToDynamicDataObject(this Exception ex)
        {
            var ret = new DynamicErrorObject("Error Details", ex.Message);
            dynamic d = ret;

            d.Type = ex.GetType().Name;

            if (ex.InnerException is null)
                return ret;

            ret.Title = "Please refer to the error property for additional information.";
            ret.Errors.Add(ex.InnerException.ToDynamicDataObject());

            return ret;
        }
    }
}
