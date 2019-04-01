using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;

namespace APIBlox.AspNetCore.Types
{
    [DebuggerDisplay("Controller: {Name} | {Route}")]
    public class DynamicControllerComposedTemplate : IComposedTemplate
    {
        public DynamicControllerComposedTemplate(DynamicAction action)
        {
            Action = action;
        }
        
        public DynamicAction Action { get; set; }
        public string Name { get; set; }
        public string Route { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is DynamicControllerComposedTemplate o))
                return false;

            return Name.EqualsEx(o.Name);
        }
    }
}
