using Microsoft.AspNetCore.Mvc;

namespace Examples.Resources
{
    /// <summary>
    ///     Class ChildByIdRequest.
    /// </summary>
    public class ChildByIdRequest
    {
        /// <summary>
        ///     Sets the child id.
        /// </summary>
        [FromRoute(Name = "childId")]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the parent identifier.
        /// </summary>
        /// <value>The parent identifier.</value>
        [FromRoute]
        public int ParentId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ChildByIdRequest"/> is help.
        /// </summary>
        /// <value><c>null</c> if [help] contains no value, <c>true</c> if [help]; otherwise, <c>false</c>.</value>
        [FromQuery] public bool? Help { get; set; }
    }
}
