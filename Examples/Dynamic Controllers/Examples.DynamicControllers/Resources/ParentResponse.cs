using System.Collections.ObjectModel;
using APIBlox.AspNetCore.Contracts;

namespace Examples.Resources
{
    /// <inheritdoc />
    internal class ParentResponse : IResource<int>
    {
        public int Id { get; set; }

        public int Age { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public Collection<ChildResponse> Children { get; set; }
    }
}
