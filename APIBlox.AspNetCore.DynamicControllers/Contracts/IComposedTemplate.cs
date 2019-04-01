using APIBlox.AspNetCore.Types;

namespace APIBlox.AspNetCore.Contracts
{
    /// <summary>
    ///     Interface IComposedTemplate
    /// </summary>
    public interface IComposedTemplate
    {
        /// <summary>
        ///     Gets the action.
        /// </summary>
        /// <value>The action.</value>
        DynamicAction Action { get; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        ///     Gets the route.
        /// </summary>
        /// <value>The route.</value>
        string Route { get; }

        /// <summary>
        ///     Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        string Namespace { get; }
    }
}
