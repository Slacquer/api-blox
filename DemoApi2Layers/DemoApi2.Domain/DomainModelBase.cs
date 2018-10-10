using System;
using System.Collections.Generic;
using APIBlox.NetCore.Contracts;
using DemoApi2.Domain.Contracts;
using Newtonsoft.Json;

namespace DemoApi2.Domain
{
    public abstract class DomainModelBase<TId> : IDomainModel<TId>
    {
        protected readonly List<IDomainEvent> EventsList = new List<IDomainEvent>();

        /// <inheritdoc />
        public DateTimeOffset CreatedOn { get; } = DateTimeOffset.Now;

        /// <inheritdoc />
        [JsonIgnore]
        public IReadOnlyCollection<IDomainEvent> Events
        {
            get
            {
                var ret = new List<IDomainEvent>(EventsList);

                EventsList.Clear();

                return ret;
            }
        }

        /// <inheritdoc />
        public TId Id { get; protected set; }

        /// <inheritdoc />
        public DateTimeOffset? ModifiedOn { get; protected set; }

        public static implicit operator bool(DomainModelBase<TId> model)
        {
            return !(model is null);
        }
    }
}
