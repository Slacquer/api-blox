using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace DemoApi2.Application.People.Queries
{
    //public class PersonQuery
    //{
    //    public int CompanyId { get; private set; }

    //    public double DepartmentId { get; private set; }

    //    public string Filter { get; private set; }
    //}

    public class PagedPersonQuery
    {
        public int CompanyId { get; private set; }

        public double DepartmentId { get; private set; }
        
        [JsonProperty(PropertyName ="Filter")]
        public string Where { get;  set; }

        public string OrderBy { get;  set; }
        
        public string Select { get;  set; }

        public int? Skip { get; set; }

        public int? Top { get;  set; }

          
    }
}
