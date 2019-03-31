using System;
using System.Collections.ObjectModel;
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
        public static RequestErrorObject SetError(
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
                : (int?)statusCode;

            return SetDetailAndAddErrorObject(error, attr.V2.ToString(), description);
        }

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
        /// <param name="title">The title.  When null, the status code will be used.</param>
        /// <param name="description">The description.</param>
        /// <returns>RequestErrorObject.</returns>
        /// <exception cref="NullReferenceException">You must create the {nameof(RequestErrorObject)}</exception>
        public static RequestErrorObject SetError(
            this RequestErrorObject error,
            int statusCode,
            string title = null,
            string description = null
        )
        {
            if (Enum.IsDefined(typeof(CommonStatusCodes), statusCode))
            {
                var enumValue = (CommonStatusCodes)statusCode;

                return SetError(error, enumValue, description);
            }

            if (error is null)
                throw new NullReferenceException(
                    $"{nameof(RequestErrorObject)} can not be null."
                );

            error.Title = title ?? statusCode.ToString();
            error.Status = statusCode;

            const string titleDetail = "Errors occured while processing request.";

            return SetDetailAndAddErrorObject(error, titleDetail, description);
        }

        /// <summary>
        ///     Sets the error to data conflict (409).
        /// </summary>
        /// <param name="error"> The <see cref="RequestErrorObject" />. </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static RequestErrorObject SetErrorToDataConflict(
            this RequestErrorObject error,
            string description = null
        )
        {
            return SetError(error, CommonStatusCodes.DataConflict, description);
        }

        /// <summary>
        ///     Sets the error to data conflict upsert (409).
        /// </summary>
        /// <param name="error"> The <see cref="RequestErrorObject" />. </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static RequestErrorObject SetErrorToDataConflictUpserts(
            this RequestErrorObject error,
            string description = null
        )
        {
            return SetError(error, CommonStatusCodes.DataConflictUpserts, description ?? "");
        }

        /// <summary>
        ///     Sets the error to forbidden (403).
        /// </summary>
        /// <param name="error"> The <see cref="RequestErrorObject" />. </param>
        /// <param name="description">The description.</param>
        public static RequestErrorObject SetErrorToForbidden(
            this RequestErrorObject error,
            string description = null
        )
        {
            return SetError(error, CommonStatusCodes.Forbidden, description);
        }

        /// <summary>
        ///     Sets the error to not found (404).
        /// </summary>
        /// <param name="error"> The <see cref="RequestErrorObject" />. </param>
        /// <param name="description">The description.</param>
        /// <returns>CommonResponse.</returns>
        public static RequestErrorObject SetErrorToNotFound(
            this RequestErrorObject error,
            string description = null
        )
        {
            return SetError(error, CommonStatusCodes.NotFound, description);
        }

        /// <summary>
        ///     Sets the error to un authorized (401).
        /// </summary>
        /// <param name="error"> The <see cref="RequestErrorObject" />. </param>
        /// <param name="description">The description.</param>
        public static RequestErrorObject SetErrorToUnAuthorized(
            this RequestErrorObject error,
            string description = null
        )
        {
            return SetError(error, CommonStatusCodes.UnAuthorized, description);
        }

        /// <summary>
        ///     Sets the error to bad request.
        /// </summary>
        /// <param name="error"> The <see cref="RequestErrorObject" />. </param>
        /// <param name="description">The description.</param>
        public static RequestErrorObject SetErrorToBadRequest(
            this RequestErrorObject error,
            string description = null
        )
        {
            return SetError(error, CommonStatusCodes.BadRequest, description);
        }

        /// <summary>
        ///     Sets the error to not acceptable.
        /// </summary>
        /// <param name="error"> The <see cref="RequestErrorObject" />. </param>
        /// <param name="description">The description.</param>
        public static RequestErrorObject SetErrorToNotAcceptable(
            this RequestErrorObject error,
            string description = null
        )
        {
            return SetError(error, CommonStatusCodes.NotAcceptable, description);
        }
        
        /// <summary>
        ///     Chainable, Adds a validation error to the internal errors collection.  It will also
        ///     set the status code to <see cref="CommonStatusCodes.BadRequest"/>
        /// </summary>
        /// <param name="errorObject">The error object.</param>
        /// <param name="property">The property.</param>
        /// <param name="errorDetails">The error details.</param>
        /// <returns>RequestErrorObject.</returns>
        public static RequestErrorObject AddValidationErrorToErrors(this RequestErrorObject errorObject, string property, string errorDetails)
        {
            dynamic validationError = new DynamicErrorObject("Invalid Property Value.", errorDetails);

            validationError.Property = property;

            if (errorObject.Errors is null)
                errorObject.Errors = new Collection<DynamicErrorObject>();

            errorObject.Errors.Add(validationError);

            errorObject.Status = (int)CommonStatusCodes.BadRequest;

            return errorObject;
        }


        
        private static RequestErrorObject SetDetailAndAddErrorObject(RequestErrorObject error, string title, string description)
        {
            if (description.IsEmptyNullOrWhiteSpace())
            {
                error.Detail = title;
                return error;
            }

            error.Detail = "Please see errors property for more details";
            error.Errors.Add(new DynamicErrorObject(title, description));

            return error;
        }

    }
}
