using System;
using System.Collections.Generic;
using System.Text;
using APIBlox.AspNetCore.Contracts;

namespace APIBlox.AspNetCore.Types
{
    public class DynamicControllerComposedTemplate : IComposedTemplate
    {
        public DynamicControllerComposedTemplate(DynamicAction action, IEnumerable<string> fields)
        {
            Action = action;
            Fields = fields;
        }

        public string Name { get; }

        public string Route { get; }

        public IEnumerable<string> Fields { get; }

        public DynamicAction Action { get; }
        
    }
}
