using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class HandlerResponseExtensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class HandlerResponseExtensions
    {
        /// <summary>
        ///     Factory method to create a new <see cref="RequestErrorObject" /> for the <see cref="HandlerResponse" /> instance.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>HandlerResponse.</returns>
        [Obsolete("This method will be removed in a future release.  Consider using the other extension methods directly.", false)]
        public static RequestErrorObject CreateError(this HandlerResponse response)
        {
            response.Error = new RequestErrorObject();

            return response.Error;
        }

        /// <summary>
        ///     Sets the error to "Bad Request" (400)
        ///     <para>
        ///         The server cannot or will not process the request due to something that is perceived to be a client error
        ///         (e.g., malformed request syntax, invalid request message framing, or deceptive request routing).
        ///     </para>
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        public static HandlerResponse SetErrorTo400BadRequest(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorTo400BadRequest(description);

            return response;
        }

        /// <summary>
        ///     Sets the error "Un-authorized" (401)
        ///     <para>
        ///         The request has not been applied because it lacks valid authentication credentials for the target resource.
        ///     </para>
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        public static HandlerResponse SetErrorTo401UnAuthorized(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorTo401UnAuthorized(description);

            return response;
        }

        /// <summary>
        ///     Sets the error to "Forbidden" (403)
        ///     <para>
        ///         The server understood the request but refuses to authorize it.
        ///     </para>
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        public static HandlerResponse SetErrorTo403Forbidden(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorTo403Forbidden(description);

            return response;
        }

        /// <summary>
        ///     Sets the error to "Not Found" (404)
        ///     <para>
        ///         The origin server did not find a current representation for the target resource or is not willing to disclose
        ///         that one exists.
        ///     </para>
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static HandlerResponse SetErrorTo404NotFound(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorTo404NotFound(description);

            return response;
        }

        /// <summary>
        ///     Sets the error to "Not Acceptable" (406)
        ///     <para>
        ///         The target resource does not have a current representation that would be acceptable to the user agent,
        ///         according to the proactive negotiation header fields received in the request, and the server is unwilling to
        ///         supply a default representation.
        ///     </para>
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        public static HandlerResponse SetErrorTo406NotAcceptable(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorTo406NotAcceptable(description);

            return response;
        }

        /// <summary>
        ///     Sets the error to "Data Conflict" (409)
        ///     <para>
        ///         The request could not be completed due to a conflict with the current state of the target resource.
        ///     </para>
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static HandlerResponse SetErrorTo409DataConflict(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorTo409DataConflict(description);

            return response;
        }

        /// <summary>
        ///     Sets the error to "Data Conflict" (409)
        ///     <para>
        ///         The request could not be completed due to a conflict with the current state of the target resource as upsert
        ///         semantics are not supported.
        ///     </para>
        /// </summary>
        /// <param name="response"> The <see cref="RequestErrorObject" />. </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static HandlerResponse SetErrorTo409DataConflictUpserts(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorTo409DataConflictUpserts(description);

            return response;
        }
    }
}
