using System.Diagnostics;

namespace APIBlox.NetCore.Models
{
    public class EventModel
    {
        public object Data { get; set;}

        public string DataType { get; set; }

        public object Metadata { get;set; }
        public long Version { get; set;}
    }
}
