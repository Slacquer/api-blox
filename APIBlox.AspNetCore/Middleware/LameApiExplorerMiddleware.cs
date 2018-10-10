using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    // This was slapped together just to see what endpoints were available quickly,
    // this COULD be done much better (still without MVC) and might actually be useful :)

    internal class LameApiExplorerMiddleware
    {
        private readonly IActionDescriptorCollectionProvider _aDcp;
        private readonly ILogger<LameApiExplorerMiddleware> _log;
        private readonly RequestDelegate _next;
        private readonly string _url;

        public LameApiExplorerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            string url = "/api/lame"
        )
        {
            _log = loggerFactory.CreateLogger<LameApiExplorerMiddleware>();
            _next = next;
            _aDcp = actionDescriptorCollectionProvider;
            _url = url;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var url = context.Request.Path.ToString();

            if (!url.EqualsEx(_url) && !url.Substring(1).EqualsEx(_url))
            {
                _log.LogInformation(() => $"Skipping as {url} is not a match for {_url}");
                await _next(context).ConfigureAwait(false);

                return;
            }

            context.Response.StatusCode = (int) HttpStatusCode.OK;

            await context.Response.WriteAsync(await BuildResponseAsync().ConfigureAwait(false)).ConfigureAwait(false);
        }

        private static async Task<dynamic> LoadHtmlFileAsync()
        {
            var ass = Assembly.GetExecutingAssembly();

            // Assembly name, no dll followed by path in project, but . not slash
            var res = ass.GetManifestResourceStream("APIBlox.AspNetCore.LameFiles.HtmlDocument.json");
            string doc;

            using (var r = new StreamReader(res))
            {
                doc = await r.ReadToEndAsync().ConfigureAwait(false);
            }

            var ret = new
            {
                Document = "",
                Footer = "",
                Table = "",
                TableTd = ""
            };

            return JsonConvert.DeserializeAnonymousType(doc, ret);
        }

        private async Task<string> BuildResponseAsync()
        {
            var file = await LoadHtmlFileAsync().ConfigureAwait(false);
            var tds = new List<string>();

            var actions = _aDcp.ActionDescriptors.Items.OfType<ControllerActionDescriptor>()
                .OrderBy(cad => cad.AttributeRouteInfo.Template)
                .ToList();

            var controllers = actions.Select(a => a.ControllerName).Distinct().ToList();

            foreach (var cad in actions)
            {
                var action = $@"<label class=""{cad.ActionName.ToLower()}"">{cad.ActionName}</label>";

                tds.Add(file.TableTd.Replace("@controller", cad.ControllerName)
                    .Replace("@action", action)
                    .Replace("@routes", cad.AttributeRouteInfo.Template)
                );
            }

            var table = file.Table.Replace("@tds", string.Join("", tds));

            var footer = file.Footer
                .Replace("@controllersCount", controllers.Count.ToString())
                .Replace("@actionsCount", actions.Count.ToString());

            var body = controllers.Any()
                ? $"{table}<br>{footer}"
                : footer;

            return file.Document.Replace("@body", body);
        }
    }
}
