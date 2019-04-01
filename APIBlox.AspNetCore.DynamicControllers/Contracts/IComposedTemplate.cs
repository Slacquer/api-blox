using System;
using System.Collections.Generic;
using System.Text;
using APIBlox.AspNetCore.Types;

namespace APIBlox.AspNetCore.Contracts
{
    public interface IComposedTemplate
    {
        DynamicAction Action { get; }

        IEnumerable<string> Fields { get; }
    }
}
