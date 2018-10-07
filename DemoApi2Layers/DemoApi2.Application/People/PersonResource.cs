#region -    Using Statements    -

using System;
using APIBlox.AspNetCore.Contracts;
using DemoApi2.Application.Locations;

#endregion

namespace DemoApi2.Application.People
{
    public class PersonResource : IResource
    {
        public LocationResource Address { get; set; }
        public DateTimeOffset BirthDate { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
