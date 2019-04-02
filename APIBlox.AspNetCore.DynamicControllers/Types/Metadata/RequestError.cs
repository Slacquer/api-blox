using System.Collections.ObjectModel;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Metadata class for <see cref="DynamicErrorObject" />
    /// </summary>
    public class RequestError //: DynamicErrorObject
    {
        private RequestError()
        {
        }

        /// <summary>
        ///     [REQUIRED]
        ///     <para>
        ///         Required when <see cref="Errors" /> is not empty.
        ///     </para>
        /// </summary>
        public string Detail { get; }

        /// <summary>
        ///     Gets or sets the inner errors.
        /// </summary>
        /// <remarks>
        ///     An object containing more specific information.
        /// </remarks>
        /// <value>The inner error.</value>
        public Collection<RequestError> Errors { get; } = new Collection<RequestError>();

        /// <summary>
        ///     [REQUIRED]
        ///     <para>
        ///         A short, human-readable summary of the problem type.It SHOULD NOT change from occurrence to occurrence
        ///         of the problem, except for purposes of localization(e.g., using proactive content negotiation;
        ///         see[RFC7231], Section 3.4).
        ///     </para>
        /// </summary>
        public string Title { get; }
    }
}
