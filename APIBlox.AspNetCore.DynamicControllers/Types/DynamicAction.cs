using System.Diagnostics;

namespace APIBlox.AspNetCore.Types
{
    [DebuggerDisplay("Action: {Name} | {Route}")]
    public class DynamicAction
    {
        public string Name { get; set; }
        public string Route { get; set; }
        public string Content { get; set; }
        public string Ctor { get; set; }
        public string[] Fields { get; set; }
        public string[] Namespaces { get; set; }
        
    }
}