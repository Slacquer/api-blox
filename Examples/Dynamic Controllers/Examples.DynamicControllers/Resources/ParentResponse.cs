#region -    Using Statements    -

using System.Collections.ObjectModel;
using APIBlox.AspNetCore.Contracts;

#endregion

namespace Examples.Resources
{
    /// <summary>
    ///     Class ParentResponse.
    /// </summary>
    /// <inheritdoc />
    public class ParentResponse : IResource<int>
    {
        /// <summary>
        ///     Gets or sets the Id.
        /// </summary>
        /// <value>The Id.</value>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the age.
        /// </summary>
        /// <value>The age.</value>
        public int Age { get; set; }

        /// <summary>
        ///     Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }

        /// <summary>
        ///     Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        public Collection<ChildResponse> Children { get; set; }
    }
}
