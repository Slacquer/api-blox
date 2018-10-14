using System.Collections.Generic;
using APIBlox.AspNetCore.Services;
using APIBlox.AspNetCore.Types;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace DemoApi2.Application.People.Queries
{
    public class PagedPersonQuery : FilteredPaginationQuery
    {
        public int CompanyId { get; private set; }

        public double DepartmentId { get; private set; }
        
    }

    //public class PagedPersonQuery
    //{
    //    public int CompanyId { get; private set; }

    //    public double DepartmentId { get; private set; }

    //    [JsonProperty(PropertyName ="Filter")]
    //    public string Where { get;  set; }

    //    public string OrderBy { get;  set; }

    //    public string Select { get;  set; }

    //    public int? Skip { get; set; }

    //    public int? Top { get;  set; }


    //}
}
