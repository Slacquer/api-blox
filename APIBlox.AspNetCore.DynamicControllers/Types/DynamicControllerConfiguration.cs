using System;
using System.Collections.Generic;
using System.Diagnostics;
using APIBlox.AspNetCore.Contracts;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    // ReSharper disable once UseNameofExpression
    [DebuggerDisplay("Internal Controller Name: {InternalControllerName}")]
    internal class DynamicControllerConfiguration : IDynamicControllerConfiguration
    {
        public DynamicControllerConfiguration(
            string controllerName, Type requestResourceType,
            Type parentIdType, params string[] routes
        )
        {
            Routes.AddRange(routes);
            RequestResourceType = requestResourceType;
            ParentIdType = parentIdType;
            ControllerName = controllerName;
        }

        public string ControllerName { get; }

        public Type ControllerType { get; set; }

        public string InternalControllerName { get; set; }

        public DynamicControllerConfiguration ParentController { get; set; }

        public Type ParentIdType { get; }

        public Type RequestResourceType { get; }

        public List<string> Routes { get; } = new List<string>();

        public List<DynamicControllerConfiguration> SubResourceControllers { get; set; } =
            new List<DynamicControllerConfiguration>();

        public static implicit operator bool(DynamicControllerConfiguration bits)
        {
            return !(bits is null);
        }
    }
}
