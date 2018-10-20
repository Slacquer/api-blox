using System;
using APIBlox.AspNetCore.Enums;
using APIBlox.AspNetCore.Types.Errors;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Extensions;

namespace APIBlox.AspNetCore.Extensions
{
    /// <summary>
    ///     Class CommonResponseExtensions.
    /// </summary>
    public static class RequestErrorObjectExtensions
    {
        /// <summary>
        ///     Sets the error with some defaults.
        ///     <para>
        ///         Use this when the other extension methods do not do what you need.
        ///     </para>
        /// </summary>
        /// <param name="error">
        ///     The <see cref="RequestErrorObject" />.
        /// </param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="description">The description.</param>
        /// <exception cref="NullReferenceException">You must create the {nameof(RequestErrorObject)}</exception>
        public static void SetError(
            this RequestErrorObject error, CommonStatusCodes statusCode,
            string description = null
        )
        {
            if (error is null)
                throw new NullReferenceException(
                    $"{nameof(RequestErrorObject)} can not be null."
                );

            var attr = statusCode.GetAttributeOfType<MetadataAttribute>();

            error.Title = attr.V1.ToString();
            error.Status = statusCode == CommonStatusCodes.DataConflictUpserts 
                ? 409 
                : (int?) statusCode;

            if (description.IsEmptyNullOrWhiteSpace())
            {
                error.Detail = attr.V2.ToString();
                return;
            }

            error.Detail = "Please see errors property for more details";
            error.Errors.Add(new DynamicErrorObject(attr.V2.ToString(), description));
        }

        /// <summary>
        ///     Sets the error to data conflict (409).
        /// </summary>
        /// <param name="error">
        ///     The <see cref="RequestErrorObject" />.  When null, one will be created.  Either way, properties
        ///     will be reset.
        /// </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static void SetErrorToDataConflict(
            this RequestErrorObject error,
            string description = null
        )
        {
            SetError(error, CommonStatusCodes.DataConflict, description);
        }

        /// <summary>
        ///     Sets the error to data conflict upsert (409).
        /// </summary>
        /// <param name="error">
        ///     The <see cref="RequestErrorObject" />.  When null, one will be created.  Either way, properties
        ///     will be reset.
        /// </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static void SetErrorToDataConflictUpserts(
            this RequestErrorObject error,
            string description = null
        )
        {
            SetError(error, CommonStatusCodes.DataConflictUpserts, description ?? "");
        }

        /// <summary>
        ///     Sets the error to forbidden (403).
        /// </summary>
        /// <param name="error">
        ///     The <see cref="RequestErrorObject" />.  When null, one will be created.  Either way, properties
        ///     will be reset.
        /// </param>
        /// <param name="description">The description.</param>
        public static void SetErrorToForbidden(
            this RequestErrorObject error,
            string description = null
        )
        {
            SetError(error, CommonStatusCodes.Forbidden, description);
        }

        /// <summary>
        ///     Sets the error to not found (404).
        /// </summary>
        /// <param name="error">
        ///     The <see cref="RequestErrorObject" />.  When null, one will be created.  Either way, properties
        ///     will be reset.
        /// </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static void SetErrorToNotFound(
            this RequestErrorObject error,
            string description = null
        )
        {
            SetError(error, CommonStatusCodes.NotFound, description);
        }

        /// <summary>
        ///     Sets the error to un authorized.
        /// </summary>
        /// <param name="error">
        ///     The <see cref="RequestErrorObject" />.  When null, one will be created.  Either way, properties
        ///     will be reset.
        /// </param>
        /// <param name="description">The description.</param>
        public static void SetErrorToUnAuthorized(
            this RequestErrorObject error,
            string description = null
        )
        {
            SetError(error, CommonStatusCodes.UnAuthorized, description);
        }
    }
}
