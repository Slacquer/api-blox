using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Services;
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
    internal class PaginationMetadataBuilder : IPaginationMetadataBuilder
    {
        private readonly int _defaultPageSize;

        public PaginationMetadataBuilder(int defaultPageSize = 1000)
        {
            _defaultPageSize = defaultPageSize;
        }

        public PaginationMetadata Build(int resultCount, ActionExecutingContext context)
        {
            // if there were query params for paging, then we must use them.
            // if there weren't then this is the first call and we will create them.
            var ret = new PaginationMetadata();

            if (context is null)
                throw new ArgumentNullException(nameof(context), "Incoming context is empty!");

            var req = context.HttpContext.Request;
            var requestQuery = BuildFromQueryParams(req.Query);

            if (requestQuery is null)
                return ret;

            var url = $"{req.Scheme}://{req.Host}{req.PathBase}{req.Path}{{0}}";

            return BuildResponseFromQuery(requestQuery, resultCount, url);
        }

        private PaginationMetadata BuildResponseFromQuery(PaginationQuery requestQuery, int count, string baseUrl)
        {
            PaginationQuery previousQuery = null;
            PaginationQuery nextQuery = null;

            if (!(requestQuery.Skip is null) && requestQuery.Skip != 0)
            {
                var previousSkip = GetPreviousSkip(requestQuery);

                previousQuery = new PaginationQuery(requestQuery)
                {
                    Skip = previousSkip
                };
            }

            var nextSkip = GetNextSkip(requestQuery);

            // If the next skip is beyond the result count, we are on the last page
            if (nextSkip < count)
                nextQuery = new PaginationQuery(requestQuery)
                {
                    Skip = nextSkip
                };

            return new PaginationMetadata
            {
                Count = count,
                Next = nextQuery is null ? null : string.Format(baseUrl, nextQuery),
                Previous = previousQuery is null ? null : string.Format(baseUrl, previousQuery)
            };
        }

        private static PaginationQuery BuildFromQueryParams(IQueryCollection requestQuery)
        {
            var query = requestQuery.Keys.ToDictionary(k => k, v => requestQuery[v].FirstOrDefault());

            var convertIncoming = JsonConvert.SerializeObject(query, Formatting.Indented, PaginationQuery.AliasesInSettings);
            var pagedQuery = JsonConvert.DeserializeObject<PaginationQuery>(convertIncoming);

            return pagedQuery;
        }

        private int? GetNextSkip(PaginationQuery query)
        {
            var top = query.Top ?? _defaultPageSize;
            var skip = query.Skip ?? 0;

            return skip + top;
        }

        private int? GetPreviousSkip(PaginationQuery query)
        {
            if (query.Skip is null)
                return null;

            var skip = query.Skip.Value;
            var top = query.Top ?? _defaultPageSize;

            int? nextSkip = skip - top;

            return nextSkip > 0 ? nextSkip : null;
        }

        private class PaginationQuery
        {
            private const string CountParam = "$count";
            private const string FilterParam = "$filter";
            private const string OrderByParam = "$orderby";
            private const string SelectParam = "$select";
            private const string SkipParam = "$skip";
            private const string TopParam = "$top";

            private static readonly Dictionary<string, string[]> InMap = new Dictionary<string, string[]>
            {
                {"Skip", new[] {"$Skip", "Offset", "$Offset"}},
                {"Top", new[] {"$Top", "Limit", "$Limit", "Take", "$Take"}},
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

            public int? Count { get; set; }

            public string Filter { get; set; }

            public string OrderBy { get; set; }

            public IEnumerable<KeyValuePair<string, StringValues>> OtherParameters { get; set; }

            public string Select { get; set; }

            public int? Skip { get; set; }

            public int? Top { get; set; }

            public override string ToString()
            {
                var qb = new QueryBuilder();

                if (Skip.HasValue)
                    qb.Add(SkipParam, Skip.Value.ToString());

                if (Top.HasValue)
                    qb.Add(TopParam, Top.Value.ToString());

                if (Count.HasValue)
                    qb.Add(CountParam, Count.Value.ToString());

                if (!OrderBy.IsEmptyNullOrWhiteSpace())
                    qb.Add(OrderByParam, OrderBy);

                if (!Filter.IsEmptyNullOrWhiteSpace())
                    qb.Add(FilterParam, Filter);

                if (!Select.IsEmptyNullOrWhiteSpace())
                    qb.Add(SelectParam, Select);

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
    }
}
