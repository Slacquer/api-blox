#region -    Using Statements    -

using APIBlox.NetCore.Attributes;
using Microsoft.AspNetCore.Http;

#endregion

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
        [Metadata("Okay", "The request completed successfully.", StatusCodes.Status200OK)]
        Ok,

        /// <summary>
        ///     204
        /// </summary>
        [Metadata("No Content", "The request input parameters yielded no results.", StatusCodes.Status204NoContent)]
        NoResults,

        /// <summary>
        ///     401
        /// </summary>
        [Metadata("Un-authorized", "You are not authenticated, meaning not authenticated at all or authenticated incorrectly.", StatusCodes.Status401Unauthorized)]
        UnAuthorized,

        /// <summary>
        ///     403
        /// </summary>
        [Metadata("Forbidden", "You do not have permission to access this resource.", StatusCodes.Status403Forbidden)]
        Forbidden,

        /// <summary>
        ///     404
        /// </summary>
        [Metadata("Not Found", "The request input parameters yielded no results.", StatusCodes.Status404NotFound)]
        NotFound,

        /// <summary>
        ///     409
        /// </summary>
        [Metadata("Data Conflict", "The request input parameters would cause a data violation.", StatusCodes.Status409Conflict)]
        DataConflict,

        /// <summary>
        ///     409
        /// </summary>
        [Metadata("Data Conflict", "The request method does not allow this functionality as upsert semantics are not supported.", StatusCodes.Status409Conflict)]
        DataConflictUpserts
    }
}
