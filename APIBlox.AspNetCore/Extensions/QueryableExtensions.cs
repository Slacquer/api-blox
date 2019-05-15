#region -    Using Statements    -

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;
using Newtonsoft.Json;

#endregion

namespace APIBlox.AspNetCore.Extensions
{
    /// <summary>
    ///     Class QueryableExtensions.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        ///     Builds results from a queryable as an asynchronous operation, that assumes client side paging.
        /// </summary>
        /// <typeparam name="TIn">The type of the t in.</typeparam>
        /// <typeparam name="TOut">The type of the t out.</typeparam>
        /// <param name="qry">The qry.</param>
        /// <param name="projection">Select projection</param>
        /// <param name="serializeSettings">Map serialize settings</param>
        /// <returns>Task&lt;List&lt;TOut&gt;&gt;.</returns>
        public static Task<List<TOut>> ResultsAsync<TIn, TOut>(this IQueryable<TIn> qry,
            Func<TIn, TOut> projection = null,
            JsonSerializerSettings serializeSettings = null
        )
            where TOut : new()
        {
            var result = qry.BuildOutputQueryable(projection, serializeSettings);

            Console.WriteLine(result.Expression.ToString());

            return result.ToDynamicListAsync<TOut>();
        }

        /// <summary>
        ///     Builds a ordered result from a queryable as an asynchronous operation, that assumes client side paging.
        /// </summary>
        /// <typeparam name="TIn">The type of the t in.</typeparam>
        /// <typeparam name="TOut">The type of the t out.</typeparam>
        /// <param name="qry">The qry.</param>
        /// <param name="request">The request.</param>
        /// <param name="projection">Select projection</param>
        /// <param name="serializeSettings">Map serialize settings</param>
        /// <returns>Task&lt;List&lt;TOut&gt;&gt;.</returns>
        public static Task<List<TOut>> OrderedResultsAsync<TIn, TOut>(this IQueryable<TIn> qry,
            IOrderedQuery request,
            Func<TIn, TOut> projection = null,
            JsonSerializerSettings serializeSettings = null
        )
            where TOut : new()
        {
            var result = qry.BuildOrderedQueryable(request)
                .BuildOutputQueryable(projection, serializeSettings);

            Console.WriteLine(result.Expression.ToString());

            return result.ToDynamicListAsync<TOut>();
        }

        /// <summary>
        ///     Builds a ordered result from a filtered queryable as an asynchronous operation, that assumes client side paging.
        /// </summary>
        /// <typeparam name="TIn">The type of the t in.</typeparam>
        /// <typeparam name="TOut">The type of the t out.</typeparam>
        /// <param name="qry">The qry.</param>
        /// <param name="request">The request.</param>
        /// <param name="projection">Select projection</param>
        /// <param name="serializeSettings">Map serialize settings</param>
        /// <returns>Task&lt;List&lt;dynamic&gt;&gt;.</returns>
        public static Task<List<dynamic>> FilteredResultsAsync<TIn, TOut>(this IQueryable<TIn> qry,
            IFilteredQuery request,
            Func<TIn, TOut> projection = null,
            JsonSerializerSettings serializeSettings = null
        )
            where TOut : new()
        {
            var tmp = qry
                .BuildOrderedQueryable(request)
                .BuildFilteredQueryable(request)
                .BuildOutputQueryable(projection, serializeSettings);

            if (request.Select.IsEmptyNullOrWhiteSpace())
            {
                Console.WriteLine(tmp.Expression.ToString());
                return tmp.ToDynamicListAsync();
            }

            var result = tmp.BuildUserProjectionQueryable(request);

            Console.WriteLine(result.Expression.ToString());

            return result.ToDynamicListAsync();
        }

        /// <summary>
        ///     Builds a paged result from a filtered queryable as an asynchronous operation, where server side paging is enforced.
        /// </summary>
        /// <typeparam name="TIn">The type of the t in.</typeparam>
        /// <typeparam name="TOut">The type of the t out.</typeparam>
        /// <param name="qry">The qry.</param>
        /// <param name="request">The request.</param>
        /// <param name="maxItemsPerResult">max items that should override the queries top.</param>
        /// <param name="projection">Select projection</param>
        /// <param name="serializeSettings">Map serialize settings</param>
        /// <returns>Task&lt;List&lt;TOut&gt;&gt;.</returns>
        public static Task<List<dynamic>> FilteredPagedResultsAsync<TIn, TOut>(this IQueryable<TIn> qry,
            FilteredPaginationQuery request, int maxItemsPerResult,
            Func<TIn, TOut> projection = null,
            JsonSerializerSettings serializeSettings = null
        )
            where TOut : new()
        {
            var tmp = qry.BuildPagedQueryable(request, maxItemsPerResult)
                .BuildOrderedQueryable(request)
                .BuildFilteredQueryable(request)
                .BuildOutputQueryable(projection, serializeSettings);

            if (request.Select.IsEmptyNullOrWhiteSpace())
            {
                Console.WriteLine(tmp.Expression.ToString());
                return tmp.ToDynamicListAsync();
            }

            var result = tmp.BuildUserProjectionQueryable(request);

            Console.WriteLine(result.Expression.ToString());

            return result.ToDynamicListAsync();
        }

        /// <summary>
        ///     Builds a paged result from a queryable as an asynchronous operation, where server side paging is enforced.
        /// </summary>
        /// <typeparam name="TIn">The type of the t in.</typeparam>
        /// <typeparam name="TOut">The type of the t out.</typeparam>
        /// <param name="qry">The qry.</param>
        /// <param name="request">The request.</param>
        /// <param name="maxItemsPerResult">max items that should override the queries top.</param>
        /// <param name="projection">Select projection</param>
        /// <param name="serializeSettings">Map serialize settings</param>
        /// <returns>Task&lt;List&lt;dynamic&gt;&gt;.</returns>
        public static Task<List<TOut>> PagedResultsAsync<TIn, TOut>(this IQueryable<TIn> qry,
            IPaginationQuery request, int maxItemsPerResult,
            Func<TIn, TOut> projection = null,
            JsonSerializerSettings serializeSettings = null
        )
            where TOut : new()
        {
            var result = qry.BuildPagedQueryable(request, maxItemsPerResult)
                .BuildOutputQueryable(projection, serializeSettings);

            Console.WriteLine(result.Expression.ToString());

            return result.ToDynamicListAsync<TOut>();
        }


        private static IQueryable<TIn> BuildFilteredQueryable<TIn>(this IQueryable<TIn> source,
            IFilteredQuery request
        )
        {
            return request.Filter.IsEmptyNullOrWhiteSpace() ? source : source.Where(request.Filter);
        }

        private static IQueryable<TIn> BuildOrderedQueryable<TIn>(this IQueryable<TIn> source,
            IOrderedQuery request
        )
        {
            return request.OrderBy.IsEmptyNullOrWhiteSpace() ? source : source.OrderBy(request.OrderBy);
        }

        private static IQueryable<TIn> BuildPagedQueryable<TIn>(this IQueryable<TIn> source,
            IPaginationQuery request, int maxItemsPerResult
        )
        {
            var max = request.Top.HasValue && request.Top.Value <= maxItemsPerResult ? request.Top.Value : maxItemsPerResult;

            source = request.Skip is null ? source : source.Skip(request.Skip.Value);
            source = source.Take(max);

            return source;
        }

        private static IQueryable BuildUserProjectionQueryable<TIn>(this IQueryable<TIn> source,
            IFilteredQuery request)
        {
            return request.Select.IsEmptyNullOrWhiteSpace() ? source : source.Select(request.Select);
        }

        private static IQueryable<TOut> BuildOutputQueryable<TIn, TOut>(this IQueryable<TIn> qry,
            Func<TIn, TOut> projection,
            JsonSerializerSettings serializeSettings
        )
            where TOut : new()
        {
            var ss = serializeSettings;

            var source = (projection is null
                    ? qry.Select(m => m.MapTo(default(TOut), ss))
                    : qry.Select(m => Projection(projection, m)))
                .AsQueryable();

            return source;
        }

        private static TOut Projection<TIn, TOut>(Func<TIn, TOut> projection, TIn m)
            where TOut : new()
        {
            return projection(m);
        }
    }
}
