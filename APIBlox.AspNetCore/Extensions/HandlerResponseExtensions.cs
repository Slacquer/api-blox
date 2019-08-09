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
        ///     Sets the error to data conflict (409).
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static HandlerResponse SetErrorToDataConflict(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorToDataConflict(description);

            return response;
        }

        /// <summary>
        ///     Sets the error to data conflict upsert (409).
        /// </summary>
        /// <param name="response"> The <see cref="RequestErrorObject" />. </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static HandlerResponse SetErrorToDataConflictUpserts(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorToDataConflictUpserts(description);

            return response;
        }

        /// <summary>
        ///     Sets the error to forbidden (403).
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        public static HandlerResponse SetErrorToForbidden(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorToForbidden(description);

            return response;
        }

        /// <summary>
        ///     Sets the error to not found (404).
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static HandlerResponse SetErrorToNotFound(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorToNotFound(description);

            return response;
        }

        /// <summary>
        ///     Sets the error to un authorized (401).
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        public static HandlerResponse SetErrorToUnAuthorized(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorToUnAuthorized(description);

            return response;
        }

        /// <summary>
        ///     Sets the error to bad request (400).
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        public static HandlerResponse SetErrorToBadRequest(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorToBadRequest(description);

            return response;
        }

        /// <summary>
        ///     Sets the error to not acceptable (406).
        /// </summary>
        /// <param name="response"> The <see cref="HandlerResponse" />. </param>
        /// <param name="description">The description.</param>
        public static HandlerResponse SetErrorToNotAcceptable(
            this HandlerResponse response,
            string description = null
        )
        {
            response.Error = new RequestErrorObject().SetErrorToNotAcceptable(description);
            
            return response;
        }
    }
}
