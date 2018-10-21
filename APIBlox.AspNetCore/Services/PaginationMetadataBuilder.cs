using System;
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

            // Throwing or should we just log.
            if (resultCount > _defaultPageSize)
                throw new IndexOutOfRangeException(
                    $"The result set of {resultCount} is larger than what " +
                    $"has been defined as the Max page size of {_defaultPageSize}."
                );

            return BuildResponseFromQuery(requestQuery, resultCount, url);
        }

        private PaginationMetadata BuildResponseFromQuery(FilteredPaginationQuery requestQuery, int resultCount, string baseUrl)
        {
            var previousRc = requestQuery.RunningCount - resultCount;

            SetRunningCount(requestQuery, resultCount);

            FilteredPaginationQuery previousQuery = null;
            FilteredPaginationQuery nextQuery = null;

            if (requestQuery.Skip.IsNullOrZero() && requestQuery.Top.IsNullOrZero())
            {
                // No inputs, create new ones, no need to new up previousQuery.
                // Set the top (take) to be the max and the skip to be what was just returned, resultCount.
                var top = GetTop(requestQuery);
                nextQuery = new FilteredPaginationQuery(requestQuery)
                {
                    Skip = requestQuery.RunningCount,
                    Top = top,
                    RunningCount = requestQuery.RunningCount
                };
            }
            else if (!requestQuery.Top.IsNullOrZero() && requestQuery.Skip.IsNullOrZero())
            {
                // Only top specified, se we use it.
                var top = GetTop(requestQuery);
                nextQuery = new FilteredPaginationQuery(requestQuery)
                {
                    Skip = requestQuery.RunningCount,
                    Top = top,
                    RunningCount = requestQuery.RunningCount
                };
            }
            else if (requestQuery.Top.HasValue)
            {
                var top = GetTop(requestQuery);
                var pre = GetPreviousSkip(requestQuery);
                var next = GetNextSkip(requestQuery);
                var nextRc = requestQuery.RunningCount;

                previousQuery = new FilteredPaginationQuery(requestQuery)
                {
                    Top = top,
                    Skip = pre,
                    RunningCount = previousRc <= 0 ? null : previousRc
                };
                nextQuery = new FilteredPaginationQuery(requestQuery)
                {
                    Top = top,
                    Skip = next,
                    RunningCount = nextRc
                };
            }

            return new PaginationMetadata
            {
                ResultCount = resultCount,
                Next = nextQuery is null ? null : string.Format(baseUrl, nextQuery),
                Previous = previousQuery is null ? null : string.Format(baseUrl, previousQuery)
            };
        }

        private static void SetRunningCount(PaginationQuery requestQuery, int resultCount)
        {
            if (!requestQuery.RunningCount.HasValue || requestQuery.Skip.IsNullOrZero() || requestQuery.Top.IsNullOrZero())
                requestQuery.RunningCount = 0;

            requestQuery.RunningCount += resultCount;
        }

        private int? GetTop(PaginationQuery requestQuery)
        {
            if (requestQuery.Top.IsNullOrZero())
                return _defaultPageSize;
            
            return requestQuery.Top > _defaultPageSize ? _defaultPageSize : requestQuery.Top;
        }

        private int? GetNextSkip(PaginationQuery query)
        {
            var top = (query.Top.IsNullOrZero() || query.Top > _defaultPageSize) ? _defaultPageSize : query.Top;
            var skip = query.Skip ?? query.RunningCount;

            return skip + top;
        }

        private int? GetPreviousSkip(PaginationQuery query)
        {
            if (query.Skip.IsNullOrZero())
                return null;

            // ReSharper disable once PossibleInvalidOperationException
            var skip = query.Skip.Value;
            var top = query.Top > _defaultPageSize ? _defaultPageSize : query.Top;

            var nextSkip = skip - top;

            return nextSkip > 0 ? nextSkip : null;
        }

        private static FilteredPaginationQuery BuildFromQueryParams(IQueryCollection requestQuery)
        {
            var query = requestQuery.Keys.ToDictionary(k => k, v => requestQuery[v].FirstOrDefault());

            var convertIncoming = JsonConvert.SerializeObject(query, Formatting.Indented, PaginationQuery.AliasesInSettings);
            var pagedQuery = JsonConvert.DeserializeObject<FilteredPaginationQuery>(convertIncoming);

            return pagedQuery;
        }
    }
}
