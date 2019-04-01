using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;

namespace APIBlox.AspNetCore.Types
{
    [DebuggerDisplay("Controller: {Name} | {Route} - Action: {Action.Name} | {Action.Route}")]
    public class DynamicControllerComposedTemplate : IComposedTemplate
    {
        public DynamicControllerComposedTemplate(string nameSpace, string route, DynamicAction action)
        {
            Namespace = nameSpace;
            Route = route;
            Action = action;
        }

        public DynamicAction Action { get; set; }
        public string Name { get; set; }
        public string Route { get;  }
        public string Namespace { get;  }
    }
}
