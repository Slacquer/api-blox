using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.NetCore.Contracts;
using Examples.EventBits;

namespace Examples.DomainModels
{
    /// <summary>
    ///     Class DomainObject.
    /// </summary>
    public class DomainObject
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="DomainObject" /> class.
        /// </summary>
        /// <param name="someValueToSave">Some value to save.</param>
        public DomainObject(int someValueToSave)
        {
            SomeValueToSave = someValueToSave;

            _domainEvents.Add(new RequestObjectCreatedEvent(someValueToSave, DateTime.Now.ToLongTimeString()));
        }

        /// <summary>
        ///     Gets some value to save.
        /// </summary>
        /// <value>Some value to save.</value>
        public int SomeValueToSave { get; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>
        ///     Gets the events.
        /// </summary>
        /// <value>The events.</value>
        public IEnumerable<IDomainEvent> Events => _domainEvents.ToList();
    }
}
