#region -    Using Statements    -

using APIBlox.AspNetCore.Contracts;

#endregion

namespace DemoApi2.Application.People
{
    public class PersonResponse : PersonResource, IResource<int>
    {
        public int Id { get; set; }
    }
}
