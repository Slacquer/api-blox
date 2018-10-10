using System;
using APIBlox.NetCore.Contracts;

namespace DemoApi2.Domain.People.Events
{
    public class PersonCreatedEvent : IDomainEvent
    {
        public PersonCreatedEvent(PersonDomainModel person)
        {
            Person = person;
        }

        /// <inheritdoc />
        public int AggregateId { get; } = new Random().Next();

        public PersonDomainModel Person { get; }
    }
}
