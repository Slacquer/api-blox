#region -    Using Statements    -

using System;
using APIBlox.NetCore.Contracts;

#endregion

namespace DemoApi2.Domain.People.Events
{
    public class PersonCreatedEvent : IDomainEvent
    {
        #region -    Constructors    -

        public PersonCreatedEvent(PersonDomainModel person)
        {
            Person = person;
        }

        #endregion

        /// <inheritdoc />
        public int AggregateId { get; } = new Random().Next();

        public PersonDomainModel Person { get; }
    }
}
