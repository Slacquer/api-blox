using System;
using System.Collections.Generic;
using APIBlox.NetCore.Extensions;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class DynamicControllerTemplateOptions.
    /// </summary>
    public class DynamicControllerTemplateOptions
    {
        private string _controllerRoute = "api/[controller]";

        /// <summary>
        ///     Gets or sets the action route.
        /// </summary>
        /// <value>The action route.</value>
        public string ActionRoute { get; set; }

        /// <summary>
        ///     Gets or sets the action comments.
        /// </summary>
        /// <value>The action comments.</value>
        public DynamicComments ActionComments { get; set; } = new();

        /// <summary>
        ///     Gets or sets the name of the controller.
        /// </summary>
        /// <value>The name of the controller.</value>
        public string ControllerName { get; set; }

        /// <summary>
        ///     Gets or sets the controller route.
        /// </summary>
        /// <remarks>
        ///     The value will be converted to camel case.
        /// </remarks>
        /// <value>The controller route.</value>
        public string ControllerRoute
        {
            get => _controllerRoute;
            set => _controllerRoute = value?.ToCamelCase();
        }

        /// <summary>
        ///     Gets or sets the controller comments.
        /// </summary>
        /// <value>The controller comments.</value>
        public DynamicComments ControllerComments { get; set; } = new();

        /// <summary>
        ///     Gets or sets the name space.
        /// </summary>
        /// <value>The name space.</value>
        public string NameSpace { get; set; } = "DynamicControllers";

        /// <summary>
        ///     Gets or sets the status codes.
        /// </summary>
        /// <value>The status codes.</value>
        public IEnumerable<int> StatusCodes { get; set; }

        /// <summary>
        ///     Sets a property, this facilitates chaining.
        /// </summary>
        /// <param name="props">The property callbacks.</param>
        /// <returns>DynamicControllerTemplateOptions.</returns>
        public DynamicControllerTemplateOptions Set(params Func<DynamicControllerTemplateOptions, object>[] props)
        {
            foreach (var prop in props)
                prop(this);

            return this;
        }
    }
}
