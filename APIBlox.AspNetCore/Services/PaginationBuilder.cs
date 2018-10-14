using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.JsonBits;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class PaginationBuilder : IPaginationBuilder
    {
        private readonly int _defaultPageSize;

        public PaginationBuilder(int defaultPageSize = 1000)
        {
            _defaultPageSize = defaultPageSize;
        }

        public PaginationResultBits Build(int resultCount, ActionExecutingContext context)
        {
            // if there were query params for paging, then we must use them.
            // if there weren't then this is the first call and we will create them.

            if (context is null)
                throw new ArgumentNullException(nameof(context), "Incoming context is empty!");

            var req = context.HttpContext.Request;
            var requestQuery = BuildFromQueryParams(req.Query);
            var responseQuery = BuildResponseQuery(requestQuery, resultCount);

            var url = $"{req.Scheme}://{req.Host}{req.PathBase}{req.Path}{{0}}";

            var ret = new PaginationResultBits
            {
                ResourceCount = resultCount,
                Next = responseQuery.Next is null ? null : string.Format(url, responseQuery.Next),
                Previous = responseQuery.Previous is null ? null : string.Format(url, responseQuery.Previous)
            };

            return ret;
        }


        private PaginationResponse BuildResponseQuery(PaginationQuery requestQuery, int count)
        {
            if (requestQuery is null)
                throw new ArgumentNullException(nameof(requestQuery));

            var previous = CreatePreviousRequest(requestQuery);
            var next = CreateNextRequest(requestQuery, count);

            return new PaginationResponse(previous, next, count);
        }

        private static PaginationQuery BuildFromQueryParams(IQueryCollection requestQuery)
        {
            var query = requestQuery.Keys.ToDictionary(k => k, v => requestQuery[v].FirstOrDefault());

            var convertIncoming = JsonConvert.SerializeObject(query, PaginationQuery.AliasesInSettings);
            var pagedQuery = JsonConvert.DeserializeObject<PaginationQuery>(convertIncoming);

            return pagedQuery;
        }

        private PaginationQuery CreateNextRequest(PaginationQuery query, long? count)
        {
            var nextSkip = GetNextSkip(query);

            // If the next skip is beyond the result count, we are on the last page
            if (nextSkip > count)
                return null;

            return TransformRequest(query, nextSkip);
        }

        private PaginationQuery CreatePreviousRequest(PaginationQuery query)
        {
            // If there is no skip we are on the first page already so no previous link
            if (query.Skip == null || query.Skip == 0)
                return null;

            var previousSkip = GetPreviousSkip(query);

            return TransformRequest(query, previousSkip);
        }

        private PaginationQuery TransformRequest(PaginationQuery request, int? newSkip)
        {
            return new PaginationQuery(request)
            {
                Skip = newSkip
            };
        }

        private int? GetNextSkip(PaginationQuery query)
        {
            var top = query.Top ?? _defaultPageSize;
            var skip = query.Skip ?? 0;

            return skip + top;
        }

        private int? GetPreviousSkip(PaginationQuery query)
        {
            if (query.Skip == null)
                return null;

            var skip = query.Skip.Value;
            var top = query.Top ?? _defaultPageSize;

            int? nextSkip = skip - top;
            return nextSkip > 0 ? nextSkip : null;
        }


        private class PaginationQuery
        {
            private const string CountParameter = "$count";
            private const string FilterParameter = "$filter";
            private const string OrderByParameter = "$orderby";
            private const string SelectParameter = "$select";
            private const string SkipParameter = "$skip";
            private const string TopParameter = "$top";

            private static readonly Dictionary<string, string[]> InMap = new Dictionary<string, string[]>
            {
                {"Skip", new[] {"$Skip", "Offset", "$Offset"}},
                {"Top", new[] {"$Top", "Limit", "$Limit"}},
                {"OrderBy", new[] {"$OrderBy"}},
                {"Filter", new[] {"$Where", "Where", "$Filter"}},
                {"Select", new[] {"$Select", "Project", "$Project"}}
            };

            public static readonly JsonSerializerSettings AliasesInSettings = new JsonSerializerSettings
            {
                ContractResolver = new AliasContractResolver(InMap)
            };

            public PaginationQuery()
            {
            }

            public PaginationQuery(PaginationQuery query)
            {
                Skip = query.Skip;
                Top = query.Top;
                OrderBy = query.OrderBy;
                Filter = query.Filter;
                Select = query.Select;
                OtherParameters = query.OtherParameters;
            }

            public string Filter { get; set; }
            public string OrderBy { get; set; }
            public IEnumerable<KeyValuePair<string, StringValues>> OtherParameters { get; set; }
            public string Select { get; set; }
            public int? Count { get; set; }
            public int? Skip { get; set; }
            public int? Top { get; set; }

            public override string ToString()
            {
                var qb = new QueryBuilder();

                if (Skip.HasValue)
                    qb.Add(SkipParameter, Skip.Value.ToString());

                if (Top.HasValue)
                    qb.Add(TopParameter, Top.Value.ToString());

                if (Count.HasValue)
                    qb.Add(CountParameter, Count.Value.ToString());

                if (!OrderBy.IsEmptyNullOrWhiteSpace())
                    qb.Add(OrderByParameter, OrderBy);

                if (!Filter.IsEmptyNullOrWhiteSpace())
                    qb.Add(FilterParameter, Filter);

                if (!Select.IsEmptyNullOrWhiteSpace())
                    qb.Add(SelectParameter, Select);

                if (OtherParameters is null)
                    return qb.ToString();

                foreach (var p in OtherParameters)
                {
                    foreach (var v in p.Value)
                        qb.Add(p.Key, v);
                }

                return qb.ToString();
            }

        }

        private class PaginationResponse
        {
            public PaginationResponse(PaginationQuery previous, PaginationQuery next, long? totalCount = null)
            {
                Previous = previous;
                Next = next;
                TotalCount = totalCount;
            }

            public PaginationQuery Next { get; }

            public PaginationQuery Previous { get; }
            private long? TotalCount { get; }
        }
    }

    internal class PaginationResultBits
    {
        public int ResourceCount { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
    }
}
