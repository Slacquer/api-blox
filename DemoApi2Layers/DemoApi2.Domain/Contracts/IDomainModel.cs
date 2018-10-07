#region -    Using Statements    -

using System;
using System.Collections.Generic;
using APIBlox.NetCore.Contracts;

#endregion

namespace DemoApi2.Domain.Contracts
{
    public interface IDomainModel<out TId>
    {
        DateTimeOffset CreatedOn { get; }

        IReadOnlyCollection<IDomainEvent> Events { get; }

        TId Id { get; }

        DateTimeOffset? ModifiedOn { get; }
    }
}
