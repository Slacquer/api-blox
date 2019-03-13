using System.Diagnostics;

namespace APIBlox.NetCore.Models
{
    public class EventModel
    {
        public object Data { get; set;}
        public object Metadata { get;set; }
        public long Version { get; set;}
        public long TimeStamp { get; set;}
    }
}
