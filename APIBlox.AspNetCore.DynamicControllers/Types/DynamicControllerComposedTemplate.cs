using System.Diagnostics;
using APIBlox.AspNetCore.Contracts;

namespace APIBlox.AspNetCore.Types
{
    /// <inheritdoc />
    /// <summary>
    ///     Class DynamicControllerComposedTemplate.
    ///     Implements the <see cref="APIBlox.AspNetCore.Contracts.IComposedTemplate" />
    /// </summary>
    [DebuggerDisplay("Controller: {Name} | {Route} - Action: {Action.Name} | {Action.Route}")]
    public class DynamicControllerComposedTemplate : IComposedTemplate
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicControllerComposedTemplate" /> class.
        /// </summary>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="route">The route.</param>
        /// <param name="action">The action.</param>
        public DynamicControllerComposedTemplate(string nameSpace, string route, DynamicAction action)
        {
            Namespace = nameSpace;
            Route = route;
            Action = action;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public DynamicAction Action { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the route.
        /// </summary>
        /// <value>The route.</value>
        public string Route { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Gets  the comments.
        /// </summary>
        /// <value>The comments.</value>
        public string Comments { get; set;}
    }
}
