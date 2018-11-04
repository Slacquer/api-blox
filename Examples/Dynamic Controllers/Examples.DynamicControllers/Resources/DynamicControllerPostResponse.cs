using APIBlox.AspNetCore.Contracts;

namespace Examples.Resources
{
    public class DynamicControllerPostResponse : IResource<int>
    {
        public int Id { get; set; }

        public string SomethingFromBody { get; set; }
    }
}
