using System;
using System.Collections.Generic;

namespace APIBlox.AspNetCore.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     Class TemplateCompilationException.
    /// </summary>
    [Serializable]
    public class TemplateCompilationException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TemplateCompilationException" /> class.
        /// </summary>
        /// <param name="compilationErrors">The compilation errors.</param>
        public TemplateCompilationException(IEnumerable<string> compilationErrors)
            : base($"Compilation Errors: \n{string.Join(Environment.NewLine, compilationErrors)}")
        {
        }
    }
}
