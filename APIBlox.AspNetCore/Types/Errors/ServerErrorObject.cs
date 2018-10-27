using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.NetCore.Extensions;
using Microsoft.Extensions.Logging;

namespace APIBlox.AspNetCore.Types.Errors
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
            {
                var msg = $"Although {GetType().Name}.{nameof(ReferenceId)} " +
                          "is not required by RFC7807, we still want it!";

                if (NoThrow)
                    Logger.LogWarning(() => msg);
                else
                    throw new ArgumentException(msg, nameof(ReferenceId));
            }
            else
                Properties.TryAdd("ReferenceId", ReferenceId);

            if (!(Errors is null) && Errors.Any())
                Properties.TryAdd("Errors", Errors);

            return base.GetDynamicMemberNames();
        }
    }
}
