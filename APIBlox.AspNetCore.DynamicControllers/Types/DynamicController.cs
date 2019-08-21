using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;

namespace APIBlox.AspNetCore.Types
{
    [DebuggerDisplay("Controller: {_name} | {_namespace} | {_route}")]
    internal class DynamicController
    {
        private static readonly string ControllerContent =
            EmbeddedResourceReader<DynamicControllerFactory>.GetResource("DynamicController.txt");

        private readonly string _controllerComments;

        private readonly string _name;
        private readonly string _namespace;
        private readonly string _route;

        public DynamicController(string name, string nameSpace, string route, string controllerComments)
        {
            _name = name;
            _namespace = nameSpace;
            _route = route;
            _controllerComments = controllerComments;

            Namespaces.Add(@"Microsoft.Extensions.Logging");
            CtorArgs.Add(@"ILoggerFactory loggerFactory");
            Fields.Add($@"private readonly ILogger<{_name}> _log;");
        }

        public List<string> Namespaces { get; } = new List<string>();

        public List<string> Fields { get; } = new List<string>();

        public List<string> CtorArgs { get; } = new List<string>();

        public List<string> CtorBody { get; } = new List<string>();

        public List<string> Actions { get; } = new List<string>();

        public List<string> Methods { get; } = new List<string>();

        public override string ToString()
        {
            var ns = string.Join("\n", Namespaces.Distinct().OrderBy(s => s).GroupBy(g => g).Select(s => $"using {s.Key.Trim()};"));
            var fields = string.Join("\n\n", Fields.Distinct().Select(s => s.Trim().EndsWith(";") ? s : $"{s};"));
            var ctorArgs = string.Join(",\n\n", CtorArgs.Where(s=> !s.IsEmptyNullOrWhiteSpace()) .Distinct());
            var ctorBody = string.Join("\n\n", CtorBody.Where(s=> !s.IsEmptyNullOrWhiteSpace()).Distinct().Select(s => s.Trim().EndsWith(";") ? s : $"{s};"));
            var actions = string.Join("\n\n", Actions.Where(s=> !s.IsEmptyNullOrWhiteSpace()).Distinct());
            var methods = string.Join("\n\n", Methods.Where(s=> !s.IsEmptyNullOrWhiteSpace()).Distinct());

            var ret = ControllerContent.Replace("[NAMESPACES]", ns)
                .Replace("[CONTROLLERS_NAMESPACE]", _namespace)
                .Replace("[CONTROLLER_NAME]", _name)
                .Replace("[CONTROLLER_REQ_OBJ_SUMMARY]", _controllerComments)
                .Replace("[CONTROLLER_ROUTE]", _route)
                .Replace("[FIELDS]", fields)
                .Replace("[CTOR_ARGS]", ctorArgs)
                .Replace("[CTOR_BODY]", ctorBody)
                .Replace("[ACTIONS]", actions)
                .Replace("[METHODS]", methods)
                .Replace(";;", ";");

            return ret;
        }
    }
}
