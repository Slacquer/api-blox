#region -    Using Statements    -

using System;
using DemoApi2.Domain.People.Events;

#endregion

namespace DemoApi2.Domain.People
{
    public sealed class PersonDomainModel : DomainModelBase<int>
    {
        #region -    Constructors    -

        /// <inheritdoc />
        public PersonDomainModel(string firstName, string lastName, DateTimeOffset dob, string emailAddress)
        {
            // https://lostechies.com/jimmybogard/2010/02/24/strengthening-your-domain-aggregate-construction/
            //
            // Not good, domain layer does NOT ref application, its the other way around.
            // So how does an entity raise an event of a specific type?
            //
            // The event object and interface must live in this assembly? ----- THIS SEEMS CORRECT!, why not? PersonCreatedEvent gets marked internal.
            // Or should the domain layer be treated like persistence, IE: dependency inversion?
            FirstName = firstName;
            LastName = lastName;
            BirthDate = dob;
            EmailAddress = emailAddress;
            Id = new Random().Next();
            EventsList.Add(new PersonCreatedEvent(this));
        }

        #endregion

        public DateTimeOffset BirthDate { get; }

        public string EmailAddress { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public override string ToString()
        {
            return $"{LastName}, {FirstName}";
        }
    }
}
