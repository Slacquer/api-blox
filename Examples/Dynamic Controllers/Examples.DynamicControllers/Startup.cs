using System.Collections.Generic;
using APIBlox.AspNetCore.Contracts;
using Examples.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Examples
{
    /// <inheritdoc />
    /// <summary>
    ///     Class Startup.
    ///     Implements the <see cref="Examples.StartupBase" />
    /// </summary>
    public class Startup : StartupBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory)
            : base(environment, loggerFactory, "ExampleControllers")
        {
        }

        /// <summary>
        ///     Builds the templates.
        /// </summary>
        /// <param name="templates">The templates.</param>
        /// <returns>IEnumerable&lt;IComposedTemplate&gt;.</returns>
        protected override IEnumerable<IComposedTemplate> BuildTemplates(List<IComposedTemplate> templates)
        {
            templates
                //.AddChildrenControllerTemplates()
                .AddParentsControllerTemplates()
                ;

            return templates;
        }
    }
}
