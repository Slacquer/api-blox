#region -    Using Statements    -

using APIBlox.AspNetCore.Contracts;

#endregion

namespace Examples.Resources
{
    /// <summary>
    ///     Class DynamicControllerResponse.
    /// </summary>
    public class DynamicControllerResponse : IResource<int>
    {
        /// <summary>
        ///     Gets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets some value that had to be three characters.
        /// </summary>
        public string SomeValueThatHadToBeThreeCharacters { get; set; }
    }
}
