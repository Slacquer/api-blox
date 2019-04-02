using System.Diagnostics;
using APIBlox.AspNetCore.Enums;
using APIBlox.AspNetCore.Extensions;
using APIBlox.AspNetCore.Types;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class HandlerResponse.
    /// </summary>
    [DebuggerStepThrough]
    public class HandlerResponse
    {
        /// <summary>
        ///     Gets the <see cref="RequestErrorObject" />, I can be set
        ///     using <see cref="RequestErrorObjectExtensions" />
        /// </summary>
        /// <value>The error.</value>
        public virtual RequestErrorObject Error { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="Error" /> is not null OR a Status that is NOT <see cref="CommonStatusCodes.Ok" />;
        ///     otherwise,<c>false</c>.
        /// </value>
        public virtual bool HasErrors => !(Error is null) || Status != CommonStatusCodes.Ok;

        /// <summary>
        ///     Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public virtual dynamic Result { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public virtual CommonStatusCodes Status { get; set; } = CommonStatusCodes.Ok;
    }
}
