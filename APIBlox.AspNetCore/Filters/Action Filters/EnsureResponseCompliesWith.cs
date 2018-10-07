#region -    Using Statements    -

using System;
using APIBlox.AspNetCore.Contracts;

#endregion

namespace APIBlox.AspNetCore.Filters
{
    internal class EnsureResponseCompliesWith : IEnsureResponseCompliesWith
    {
        public Func<object, object> Func { get; set; } = objectResultValue => new {Data = objectResultValue};
    }
}
