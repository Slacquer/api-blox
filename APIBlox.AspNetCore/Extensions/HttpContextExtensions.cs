using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace APIBlox.AspNetCore.Extensions
{
    /// <summary>
    ///     Class HttpContextExtensions.
    /// </summary>
    public static class HttpContextExtensions
    {
        private static readonly ActionDescriptor EmptyActionDescriptor = new();
        private static readonly RouteData EmptyRouteData = new();

        /// <summary>
        ///     Writes the result executor asynchronous.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="actionContext">Current action context</param>
        /// <returns>Task.</returns>
        public static Task WriteResultExecutorAsync<TResult>(this HttpContext context, TResult result, ActionContext actionContext = null)
            where TResult : IActionResult
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var executor = context.RequestServices.GetService<IActionResultExecutor<TResult>>();

            if (executor == null)
            {
                var t = typeof(TResult);
                var ns = typeof(IActionResultExecutor<>).Namespace;

                throw new InvalidOperationException(
                    $"No result executor for '{t.FullName}' has been registered.  " +
                    $"Did you add it?  IE: services.TryAddSingleton<IActionResultExecutor<{t.Name}>, " +
                    $"[One of the executors in the {ns}]ResultExecutor>()"
                );
            }

            var routeData = context.GetRouteData() ?? EmptyRouteData;

            var ac = actionContext ?? new ActionContext(context, routeData, EmptyActionDescriptor);

            return executor.ExecuteAsync(ac, result);
        }
    }
}
