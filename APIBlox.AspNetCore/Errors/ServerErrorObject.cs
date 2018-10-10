using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.NetCore.Extensions;

namespace APIBlox.AspNetCore.Errors
{
    internal class ServerErrorObject : RequestErrorObject
    {
        public ServerErrorObject(string title, string detail, int status, string instance, string referenceId)
            : base(title, detail, status, instance)
        {
            ReferenceId = referenceId;
        }

        private string ReferenceId { get; }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (ReferenceId.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException($"Although {GetType().Name}.{nameof(ReferenceId)} is not required by RFC7807, we still want it!",
                    nameof(ReferenceId)
                );

            Properties.TryAdd("ReferenceId", ReferenceId);

            if (Errors.Any())
                Properties.TryAdd("Errors", Errors);

            return base.GetDynamicMemberNames();
        }
    }
}
