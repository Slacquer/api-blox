using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using APIBlox.AspNetCore.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class PaginationBuilder : IPaginationBuilder
    {
        private readonly int defaultPageSize;


        public PaginationBuilder(int defaultPageSize = 1000)
        {
            this.defaultPageSize = defaultPageSize;
        }

        public dynamic Build(IEnumerable<object> result, ActionExecutingContext context)
        {
            // if there were query params for paging, then we must use them.
            // if there weren't then this is the first call and we will create them.

            var pagedBits = BuildFromQueryParams(context.HttpContext.Request.Query);

            pagedBits.Count = result.Count();

            return pagedBits;
        }

        private static PaginationBits BuildFromQueryParams(IQueryCollection requestQuery)
        {
            var query = requestQuery.Keys.ToDictionary(k => k, v => requestQuery[v].FirstOrDefault());
            var map = new Dictionary<Type, Dictionary<string, string>>
            {
                {
                    typeof(PaginationQuery),
                    new Dictionary<string, string>
                    {
                        
                        // should be {"Skip", new[]{"Offset","$Offset","$Skip"}}

                        {"Offset", "Skip"},
                        {"$Offset", "Skip"},
                        {"$Skip", "Skip"},

                        {"Limit", "Top"},
                        {"$Limit", "Top"},
                        {"$Top", "Top"},

                        {"$OrderBy", "OrderBy"},
                        {"$Where", "Where"},

                        
                        {"Project", "Select"},
                        {"$Project", "Select"},
                        {"$Select", "Select"},
                    }
                }
            };
            
            var settings = new JsonSerializerSettings {ContractResolver = new DynamicMappingResolver(map)};

            var pagedQuery = JsonConvert.DeserializeObject<PaginationQuery>(JsonConvert.SerializeObject(query), settings);



            return new PaginationBits();
        }

        private int? GetNextSkip(PaginationQuery query)
        {
            int top = query.Top ?? defaultPageSize;
            int skip = query.Skip ?? 0;

            return skip + top;
        }

        private int? GetPreviousSkip(PaginationQuery query)
        {
            if (query.Skip == null) return null;

            int skip = query.Skip.Value;
            int top = query.Top ?? defaultPageSize;

            int? nextSkip = skip - top;
            return (nextSkip > 0) ? nextSkip : null;
        }


        private class PaginationBits
        {
            public string Next { get; set; }

            public string Previous { get; set; }

            public int Count { get; set; }
        }

        private class PaginationQuery
        {
            //public int? Offset { get; set; }
            public int? Skip { get; set; }


            //public int? Limit { get; set; }
            public int? Top { get; set; }

            public string OrderBy { get; set; }
            public string Where { get; set; }
            public string Select { get; set; }
        }
    }

    // change me to FIND the incoming member.Name in the given dictionary.
    internal class DynamicMappingResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, Dictionary<string, string>> _memberNameToJsonNameMap;

        public DynamicMappingResolver(Dictionary<Type, Dictionary<string, string>> memberNameToJsonNameMap)
        {
            _memberNameToJsonNameMap = memberNameToJsonNameMap;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            _memberNameToJsonNameMap.TryGetValue(member.DeclaringType, out var dict);

            if (!(dict is null))
            {
                if (dict.TryGetValue(member.Name, out var jsonName))
                {
                    prop.PropertyName = jsonName;
                }
            }

            return prop;
        }
    }
    
}
