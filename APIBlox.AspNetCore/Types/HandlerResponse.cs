using System.Diagnostics;
using APIBlox.AspNetCore.Enums;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class HandlerResponse.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class HandlerResponse
    {
        /// <summary>
        ///     Gets the <see cref="RequestErrorObject" />, I can be set
        ///     using <see cref="RequestErrorObjectExtensions" />
        /// </summary>
        /// <value>The error.</value>
        public RequestErrorObject Error { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="Error" /> is not null OR a Status that is NOT <see cref="CommonStatusCodes.Status200Ok" />;
        ///     otherwise,<c>false</c>.
        /// </value>
        public bool HasErrors => !(Error is null) || Status != CommonStatusCodes.Status200Ok;

        /// <summary>
        ///     Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public dynamic Result { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public CommonStatusCodes Status { get; set; } = CommonStatusCodes.Status200Ok;
    }
}
