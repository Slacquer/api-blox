using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using Examples.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Examples
{
    public class Startup : StartupBase
    {
        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory)
            : base(environment, loggerFactory, "ExampleControllers")
        {

        }

        protected override IEnumerable<IComposedTemplate> BuildTemplates(List<IComposedTemplate> templates)
        {
            templates .AddChildrenControllerTemplates()
                .AddParentsControllerTemplates();


            return templates;
        }
    }
}
