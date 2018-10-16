using System;
using System.Linq;
using APIBlox.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Class ApplicationBuilderExtensions.
    /// </summary>
    public static class ApplicationBuilderExtensionsAspNetCore
    {
        /// <summary>
        ///     Configures the server faults middleware using <see cref="ServerFaultsMiddleware" />.
        ///     <para>
        ///         I should be used as early as possible in the configure pipeline.  Unless if using
        ///         <see cref="DeveloperExceptionPageMiddleware" /> then
        ///         be sure to use an ELSE block, otherwise I will take over, MUHAHAHA!.
        ///     </para>
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="typeUrl">The url to specify in the <see cref="ProblemDetails.Type" /></param>
        /// <param name="referenceIdFunc">
        ///     A callback function to set the reference number of the fault, by default this is
        ///     <see cref="DateTimeOffset.Now" />.Ticks
        /// </param>
        /// <returns>IApplicationBuilder.</returns>
        public static IApplicationBuilder UseServerFaults(
            this IApplicationBuilder app,
            string typeUrl = @"about:blank", Func<string> referenceIdFunc = null
        )
        {
            return app.UseExceptionHandler(c =>
                c.UseMiddleware<ServerFaultsMiddleware>(typeUrl,
                    referenceIdFunc ?? (() => DateTimeOffset.Now.Ticks.ToString())
                )
            );
        }

        /// <summary>
        ///     Uses the <see cref="SimulateWaitTimeMiddleware" />.  This only works
        ///     in dev and the request path must contain query param wait.
        ///     <para>I should be used as early in the pipeline as possible.</para>
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="env">Hosting environment, simulate only works in dev.</param>
        /// <param name="excludeUrls">Urls that contain bits to disregard</param>
        /// <returns>IApplicationBuilder.</returns>
        public static IApplicationBuilder UseSimulateWaitTime(
            this IApplicationBuilder application,
            IHostingEnvironment env,
            params string[] excludeUrls
        )
        {
            if (!env.IsDevelopment())
                return application;

            var lst = excludeUrls.ToList();

            return application.UseMiddleware<SimulateWaitTimeMiddleware>(lst);
        }
    }
}
