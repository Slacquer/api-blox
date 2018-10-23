using System.Collections.ObjectModel;
using APIBlox.AspNetCore.Contracts;

namespace Examples.Resources
{
    internal class ParentResponse : IResource<int>
    {
        public int Age { get; set; }

        public string FirstName { get; set; }
        public int Id { get; set; }

        public Collection<ChildResponse> Kids { get; set; }

        public string LastName { get; set; }
    }
}
