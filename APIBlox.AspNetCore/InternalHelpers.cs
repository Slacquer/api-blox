#region -    Using Statements    -

using System;
using APIBlox.AspNetCore.Errors;

#endregion

namespace APIBlox.AspNetCore
{
    internal static class InternalHelpers
    {
        public static Action<RequestErrorObject> AlterRequestErrorObjectAction { get; set; }

        public static Func<object, object> EnsureResponseCompliesWithAction { get; set; } = objectResultValue => new {Data = objectResultValue};
    }
}
