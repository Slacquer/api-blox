﻿using System.Collections.Generic;
using System.Linq;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;

namespace APIBlox.AspNetCore.Types
{
    internal class DynamicController
    {
        private static readonly string ControllerDefaultContent = EmbeddedResourceReader<DynamicControllerFactory>.GetResource("DynamicController.txt");

        public DynamicController(string name, string nameSpace, string route)
        {
            Content = ControllerDefaultContent;
            Name = name;
            NameSpace = nameSpace;
            Route = route;
        }

        public string Name { get; }
        public string NameSpace { get; }

        public string Route { get; }

        public string Content { get; }

        public List<string> Namespaces { get; set; } = new List<string>();

        public List<string> Fields { get; set; } = new List<string>();

        public List<string> Ctors { get; set; } = new List<string>();

        public List<string> Actions { get; set; } = new List<string>();

        public override string ToString()
        {

            var ns = string.Join("\n", Namespaces.Select(s => $"using {s.Trim()};"));
            var fields = string.Join("\n\n", Fields);
            var ctors = string.Join("\n\n", Ctors);
            var actions = string.Join("\n\n", Actions);

            var ret = Content.Replace("[NAMESPACES]", ns)
                .Replace("[CONTROLLERS_NAMESPACE]", NameSpace)
                .Replace("[CONTROLLER_NAME]", Name)
                .Replace("[CONTROLLER_ROUTE]", Route)
                .Replace("[FIELDS]", fields)
                .Replace("[CTORS]", ctors)
                .Replace("[ACTIONS]", actions);

            return ret;
        }
    }
}
