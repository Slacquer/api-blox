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
        [Metadata("Okay", "The request completed successfully.")]
        Ok = StatusCodes.Status200OK,

        /// <summary>
        ///     204
        /// </summary>
        [Metadata("No Content", "The request input parameters yielded no results.")]
        NoResults = StatusCodes.Status204NoContent,

        /// <summary>
        ///     401
        /// </summary>
        [Metadata("Un-authorized", "You are not authenticated, meaning not authenticated at all or authenticated incorrectly.")]
        UnAuthorized = StatusCodes.Status401Unauthorized,

        /// <summary>
        ///     403
        /// </summary>
        [Metadata("Forbidden", "You do not have permission to access this resource.")]
        Forbidden = StatusCodes.Status403Forbidden,

        /// <summary>
        ///     404
        /// </summary>
        [Metadata("Not Found", "The request input parameters yielded no results.")]
        NotFound = StatusCodes.Status404NotFound,

        /// <summary>
        ///     409
        /// </summary>
        [Metadata("Data Conflict", "The request input parameters would cause a data violation.")]
        DataConflict = StatusCodes.Status409Conflict,

        /// <summary>
        ///     409
        /// </summary>
        [Metadata("Data Conflict", "The request method does not allow this functionality as upsert semantics are not supported.")]
        DataConflictUpserts = 4091
    }
}
