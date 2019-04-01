using System;
using System.Collections.Generic;
using System.Text;
using APIBlox.AspNetCore.Types;

namespace APIBlox.AspNetCore.Contracts
{
    public interface IComposedTemplate
    {
        DynamicAction Action { get; }

         string Name { get; set; }
         string Route { get; set; }
    }
}
