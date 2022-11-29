using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Services;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class PaginationMetadataBuilder : IPaginationMetadataBuilder
    {
        private Dictionary<string, string> _queryParams;

        public PaginationMetadataBuilder(List<string> routes = null)
        {
            Routes = routes ?? new List<string>();
        }

        public List<string> Routes { get; }

        public PaginationMetadata Build(int resultCount, ActionExecutingContext context)
        {
            // if there were query params for paging, then we must use them.
            // if there weren't then this is the first call and we will create them.

            if (context is null)
                throw new ArgumentNullException(nameof(context), "Incoming context is empty!");

            var req = context.HttpContext.Request;
            _queryParams = req.Query.Keys.ToDictionary(k => k, v => req.Query[v].FirstOrDefault());

            var url = $"{req.Scheme}://{req.Host}{req.PathBase}{req.Path}{{0}}";

            return BuildResponseFromQuery(resultCount, url);
        }

        private PaginationMetadata BuildResponseFromQuery(int resultCount, string baseUrl)
        {
            var requestQuery = new FilteredPaginationQuery();
            requestQuery.SetAliasesAndValues(_queryParams);

            var maxPageSize = (requestQuery.Top.IsNullOrDefault()
                ? requestQuery.Skip.IsNullOrDefault()
                    ? resultCount
                    : requestQuery.Skip
                : requestQuery.Top) ?? resultCount;

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

            if (requestQuery.Skip.IsNullOrDefault() && requestQuery.Top.IsNullOrDefault())
            {
                // No inputs, create new ones, no need to new up previousQuery.
                // Set the top (take) to be the max and the skip to be what was just returned, resultCount.
                var top = GetTop(requestQuery, maxPageSize);

                nextQuery = Clone(requestQuery);
                nextQuery.Skip = requestQuery.RunningCount;
                nextQuery.Top = top;
                nextQuery.RunningCount = requestQuery.RunningCount;
            }
            else if (!requestQuery.Top.IsNullOrDefault() && requestQuery.Skip.IsNullOrDefault())
            {
                // Only top specified, se we use it.
                var top = GetTop(requestQuery, maxPageSize);

                nextQuery = Clone(requestQuery);
                nextQuery.Skip = requestQuery.RunningCount;
                nextQuery.Top = top;
                nextQuery.RunningCount = requestQuery.RunningCount;
            }
            else if (requestQuery.Top.HasValue)
            {
                var top = GetTop(requestQuery, maxPageSize);
                var pre = GetPreviousSkip(requestQuery, maxPageSize);
                var next = GetNextSkip(requestQuery, maxPageSize);
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

            if (nextQuery is not null)
                nextQuery.Undefined = requestQuery.Undefined;

            if (previousQuery is not null)
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

            // if previous is empty and we do not have more than top, then next should be null as well.
            if (ret.Previous is not null)
                return ret;

            if (nextQuery is not null && resultCount < nextQuery.Top)
                ret.Next = null;

            return ret;
        }

        private FilteredPaginationQuery Clone(FilteredPaginationQuery org)
        {
            var ret = JsonConvert.DeserializeObject<FilteredPaginationQuery>(JsonConvert.SerializeObject(org))!;

            ret.SetAliasesAndValues(_queryParams);

            return ret;
        }

        private static void SetRunningCount(IPaginationQuery requestQuery, int resultCount)
        {
            if (!requestQuery.RunningCount.HasValue || requestQuery.Skip.IsNullOrDefault() || requestQuery.Top.IsNullOrDefault())
                requestQuery.RunningCount = 0;

            requestQuery.RunningCount += resultCount;
        }

        private static int? GetTop(IPaginationQuery requestQuery, int defaultPageSize)
        {
            if (requestQuery.Top.IsNullOrDefault())
                return defaultPageSize;

            return requestQuery.Top > defaultPageSize ? defaultPageSize : requestQuery.Top;
        }

        private static int? GetNextSkip(IPaginationQuery query, int defaultPageSize)
        {
            var top = query.Top.IsNullOrDefault() || query.Top > defaultPageSize ? defaultPageSize : query.Top;
            var skip = query.Skip ?? query.RunningCount;

            return skip + top;
        }

        private static int? GetPreviousSkip(IPaginationQuery query, int defaultPageSize)
        {
            if (query.Skip.IsNullOrDefault())
                return null;

            // ReSharper disable once PossibleInvalidOperationException
            var skip = query.Skip.Value;
            var top = query.Top > defaultPageSize ? defaultPageSize : query.Top;

            var nextSkip = skip - top;

            return nextSkip > 0 ? nextSkip : null;
        }
    }
}
