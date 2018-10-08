#region -    Using Statements    -

using Microsoft.AspNetCore.JsonPatch;

#endregion

namespace APIBlox.AspNetCore.RequestsResponses
{
    /// <summary>
    ///     Class PatchRequest.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    public class PatchRequest<TRequest>
        where TRequest : class
    {
        /// <summary>
        ///     Gets or sets the patch.
        /// </summary>
        /// <value>The patch.</value>
        public JsonPatchDocument<TRequest> Patch { get; set; }
    }

    /// <summary>
    ///     Class PatchRequest.
    /// </summary>
    public class PatchRequest
    {
        /// <summary>
        ///     Gets or sets the patch.
        /// </summary>
        /// <value>The patch.</value>
        public JsonPatchDocument Patch { get; set; }
    }
}
