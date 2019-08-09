using APIBlox.NetCore.Attributes;
using Microsoft.AspNetCore.Http;

namespace APIBlox.AspNetCore.Enums
{
    /// <summary>
    ///     Enum CommonStatusCodes
    /// </summary>
    public enum CommonStatusCodes
    {
        /// <summary>
        ///     200
        /// </summary>
        [Metadata("Okay", "The request has succeeded.")]
        Status200Ok = StatusCodes.Status200OK,

        /// <summary>
        ///     201
        /// </summary>
        [Metadata("Created", "The request has been fulfilled and has resulted in one or more new resources being created.")]
        Status201Created = StatusCodes.Status201Created,

        /// <summary>
        ///     202
        /// </summary>
        [Metadata("Accepted", "The request has been accepted for processing, but the processing has not been completed. The request might or might not eventually be acted upon, as it might be disallowed when processing actually takes place.")]
        Status202Accepted = StatusCodes.Status202Accepted,

        /// <summary>
        ///     204
        /// </summary>
        [Metadata("No Content", "The server has successfully fulfilled the request and that there is no additional content to send in the response payload body.")]
        Status204NoContent = StatusCodes.Status204NoContent,

        /// <summary>
        ///     400
        /// </summary>
        [Metadata("Bad Request", "The server cannot or will not process the request due to something that is perceived to be a client error (e.g., malformed request syntax, invalid request message framing, or deceptive request routing).")]
        Status400BadRequest = StatusCodes.Status400BadRequest,

        /// <summary>
        ///     401
        /// </summary>
        [Metadata("Un-authorized", "The request has not been applied because it lacks valid authentication credentials for the target resource.")]
        Status401Unauthorized = StatusCodes.Status401Unauthorized,

        /// <summary>
        ///     403
        /// </summary>
        [Metadata("Forbidden", "The server understood the request but refuses to authorize it.")]
        Status403Forbidden = StatusCodes.Status403Forbidden,

        /// <summary>
        ///     404
        /// </summary>
        [Metadata("Not Found", "The origin server did not find a current representation for the target resource or is not willing to disclose that one exists.")]
        Status404NotFound = StatusCodes.Status404NotFound,

        /// <summary>
        ///     406
        /// </summary>
        [Metadata("NotAcceptable", "The target resource does not have a current representation that would be acceptable to the user agent, according to the proactive negotiation header fields received in the request, and the server is unwilling to supply a default representation.")]
        Status406NotAcceptable = StatusCodes.Status406NotAcceptable,

        /// <summary>
        ///     409
        /// </summary>
        [Metadata("Data Conflict", "The request could not be completed due to a conflict with the current state of the target resource.")]
        Status409Conflict = StatusCodes.Status409Conflict,

        /// <summary>
        ///     409
        /// </summary>
        [Metadata("Data Conflict", "The request could not be completed due to a conflict with the current state of the target resource as upsert semantics are not supported.")]
        Status409ConflictUpserts = 4091
    }
}
