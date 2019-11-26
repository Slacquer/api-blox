using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Services;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class PaginationMetadataBuilder : IPaginationMetadataBuilder
    {
        private readonly int _defaultPageSize;
        private Dictionary<string, string> _queryParams;

        public PaginationMetadataBuilder(int defaultPageSize = 1000, Dictionary<string, int> routePageSizes = null)
        {
            _defaultPageSize = defaultPageSize;

            RoutePageSizes = routePageSizes ?? new Dictionary<string, int>();
        }

        public Dictionary<string, int> RoutePageSizes { get; }

        public PaginationMetadata Build(int resultCount, ActionExecutingContext context)
        {
            // if there were query params for paging, then we must use them.
            // if there weren't then this is the first call and we will create them.

            if (context is null)
                throw new ArgumentNullException(nameof(context), "Incoming context is empty!");

            var req = context.HttpContext.Request;
            _queryParams = req.Query.Keys.ToDictionary(k => k, v => req.Query[v].FirstOrDefault());

            var url = $"{req.Scheme}://{req.Host}{req.PathBase}{req.Path}{{0}}";

            var maxPageSize = GetMaxPageSize(req.Path);

            return BuildResponseFromQuery(resultCount, url, maxPageSize);
        }

        private int GetMaxPageSize(PathString reqPath)
        {
            if (!RoutePageSizes.Any())
                return _defaultPageSize;

            var p = reqPath.ToString().ToLowerInvariant();

            if (!RoutePageSizes.ContainsKey(p))
                return _defaultPageSize;

            var max = RoutePageSizes[p];

            return max;
        }

        private PaginationMetadata BuildResponseFromQuery(int resultCount, string baseUrl, int maxPageSize)
        {
            var requestQuery = new FilteredPaginationQuery();
            requestQuery.SetAliasesAndValues(_queryParams);

            // If resultCount is 0 or empty then we are just going to display the structure.
            // If resultCount is less than the max, and no query params have been passed then no need to have anything either.
            if (resultCount == 0 || resultCount < maxPageSize && !_queryParams.Any())
                return new PaginationMetadata
                {
                    ResultCount = resultCount
                };

            var previousRc = requestQuery.RunningCount - resultCount;

            SetRunningCount(requestQuery, resultCount);

            FilteredPaginationQuery previousQuery = null;
            FilteredPaginationQuery nextQuery = null;

            if (requestQuery.Skip.IsNullOrZero() && requestQuery.Top.IsNullOrZero())
            {
                // No inputs, create new ones, no need to new up previousQuery.
                // Set the top (take) to be the max and the skip to be what was just returned, resultCount.
                var top = GetTop(requestQuery);

                nextQuery = Clone(requestQuery);
                nextQuery.Skip = requestQuery.RunningCount;
                nextQuery.Top = top;
                nextQuery.RunningCount = requestQuery.RunningCount;
            }
            else if (!requestQuery.Top.IsNullOrZero() && requestQuery.Skip.IsNullOrZero())
            {
                // Only top specified, se we use it.
                var top = GetTop(requestQuery);

                nextQuery = Clone(requestQuery);
                nextQuery.Skip = requestQuery.RunningCount;
                nextQuery.Top = top;
                nextQuery.RunningCount = requestQuery.RunningCount;
            }
            else if (requestQuery.Top.HasValue)
            {
                var top = GetTop(requestQuery);
                var pre = GetPreviousSkip(requestQuery);
                var next = GetNextSkip(requestQuery);
                var nextRc = requestQuery.RunningCount;

                previousQuery = Clone(requestQuery);
                previousQuery.Top = top;
                previousQuery.Skip = pre;
                previousQuery.RunningCount = previousRc <= 0 ? null : previousRc;

                // Do we need next?
                if (resultCount >= maxPageSize)
                {
                    nextQuery = Clone(requestQuery);
                    nextQuery.Skip = next;
                    nextQuery.Top = top;
                    nextQuery.RunningCount = nextRc;
                }
            }

            if (!(nextQuery is null))
                nextQuery.Undefined = requestQuery.Undefined;

            if (!(previousQuery is null))
                previousQuery.Undefined = previousQuery.Undefined;

            var ret = new PaginationMetadata
            {
                ResultCount = resultCount,
                Next = nextQuery is null
                    ? null
                    : string.Format(baseUrl, nextQuery),
                Previous = previousQuery is null
                    ? null
                    : string.Format(baseUrl, previousQuery)
            };

            // if previous is empty and we do not have more than max, then next should be null as well.
            if (ret.Previous is null && resultCount < maxPageSize)
                ret.Next = null;

            return ret;
        }

        private FilteredPaginationQuery Clone(FilteredPaginationQuery org)
        {
            var ret = JsonConvert.DeserializeObject<FilteredPaginationQuery>(JsonConvert.SerializeObject(org));

            ret.SetAliasesAndValues(_queryParams);

            return ret;
        }

        private static void SetRunningCount(IPaginationQuery requestQuery, int resultCount)
        {
            if (!requestQuery.RunningCount.HasValue || requestQuery.Skip.IsNullOrZero() || requestQuery.Top.IsNullOrZero())
                requestQuery.RunningCount = 0;

            requestQuery.RunningCount += resultCount;
        }

        private int? GetTop(IPaginationQuery requestQuery)
        {
            if (requestQuery.Top.IsNullOrZero())
                return _defaultPageSize;

            return requestQuery.Top > _defaultPageSize ? _defaultPageSize : requestQuery.Top;
        }

        private int? GetNextSkip(IPaginationQuery query)
        {
            var top = query.Top.IsNullOrZero() || query.Top > _defaultPageSize ? _defaultPageSize : query.Top;
            var skip = query.Skip ?? query.RunningCount;

            return skip + top;
        }

        private int? GetPreviousSkip(IPaginationQuery query)
        {
            if (query.Skip.IsNullOrZero())
                return null;

            // ReSharper disable once PossibleInvalidOperationException
            var skip = query.Skip.Value;
            var top = query.Top > _defaultPageSize ? _defaultPageSize : query.Top;

            var nextSkip = skip - top;

            return nextSkip > 0 ? nextSkip : null;
        }
    }
}
