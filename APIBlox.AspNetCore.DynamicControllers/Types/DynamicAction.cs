using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class DynamicAction.
    /// </summary>
    [DebuggerDisplay("Action: {Name} | {Route}")]
    public class DynamicAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicAction" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="route">The route.</param>
        /// <param name="content">The content.</param>
        /// <param name="ctor">The ctor.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="namespaces">The namespaces.</param>
        /// <param name="methods">The methods.</param>
        public DynamicAction(string name, string route, string content, string ctor, string[] fields, string[] namespaces, string[] methods = null)
        {
            Name = name;
            Route = route;
            Content = content;
            Ctor = ctor;
            Fields = fields;
            Namespaces = namespaces;
            Methods = methods;
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        ///     Gets the route.
        /// </summary>
        /// <value>The route.</value>
        public string Route { get; }

        /// <summary>
        ///     Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; private set; }

        /// <summary>
        ///     Gets the ctor.
        /// </summary>
        /// <value>The ctor.</value>
        public string Ctor { get; private set; }

        /// <summary>
        ///     Gets the fields.
        /// </summary>
        /// <value>The fields.</value>
        public string[] Fields { get; private set; }

        /// <summary>
        ///     Gets or sets the namespaces.
        /// </summary>
        /// <value>The namespaces.</value>
        public string[] Namespaces { get; set; }

        /// <summary>
        ///     Gets the methods.
        /// </summary>
        /// <value>The methods.</value>
        public string[] Methods { get; private set; }

        /// <summary>
        ///     Gets the tokens.
        /// </summary>
        /// <value>The tokens.</value>
        public Dictionary<string, string> Tokens { get; } = new Dictionary<string, string>
        {
            {"[REQ_OBJECT]", ""},
            {"[RES_OBJECT_INNER_RESULT]", ""},
            {"[ACTION_ROUTE]", ""},
            {"[PARAMS_COMMENTS]", ""},
            {"[RES_OBJECT_RESULT]", ""},
            {"[ACTION_PARAMS]", ""},
            {"[NEW_REQ_OBJECT]", ""},
            {"[CONTROLLER_NAME]", ""}
        };

        /// <summary>
        ///     Composes this instance.
        /// </summary>
        public void Compose()
        {
            Content = Content
                .Replace("[REQ_OBJECT]", Tokens["[REQ_OBJECT]"])
                .Replace("[RES_OBJECT_INNER_RESULT]", Tokens["[RES_OBJECT_INNER_RESULT]"])
                .Replace("[ACTION_ROUTE]", Tokens["[ACTION_ROUTE]"])
                .Replace("[PARAMS_COMMENTS]", Tokens["[PARAMS_COMMENTS]"])
                .Replace("[RES_OBJECT_RESULT]", Tokens["[RES_OBJECT_RESULT]"])
                .Replace("[ACTION_PARAMS]", Tokens["[ACTION_PARAMS]"])
                .Replace("[NEW_REQ_OBJECT]", Tokens["[NEW_REQ_OBJECT]"])
                .Replace("()]", "]")
                .Replace("(\"\")", "");

            Ctor = Ctor
                .Replace("[REQ_OBJECT]", Tokens["[REQ_OBJECT]"])
                .Replace("[RES_OBJECT_INNER_RESULT]", Tokens["[RES_OBJECT_INNER_RESULT]"])
                .Replace("[CONTROLLER_NAME]", Tokens["[CONTROLLER_NAME]"]);

            Fields = Fields.Select(s =>
                s.Replace("[REQ_OBJECT]", Tokens["[REQ_OBJECT]"])
                    .Replace("[RES_OBJECT_INNER_RESULT]", Tokens["[RES_OBJECT_INNER_RESULT]"])
            ).ToArray();

            if (Methods is null || !Methods.Any())
                return;

            Methods = Methods.Select(s =>
                s.Replace("[REQ_OBJECT]", Tokens["[REQ_OBJECT]"])
                    .Replace("[RES_OBJECT_INNER_RESULT]", Tokens["[RES_OBJECT_INNER_RESULT]"])
                    .Replace("[ACTION_ROUTE]", Tokens["[ACTION_ROUTE]"])
                    .Replace("[PARAMS_COMMENTS]", Tokens["[PARAMS_COMMENTS]"])
                    .Replace("[RES_OBJECT_RESULT]", Tokens["[RES_OBJECT_RESULT]"])
                    .Replace("[ACTION_PARAMS]", Tokens["[ACTION_PARAMS]"])
                    .Replace("[NEW_REQ_OBJECT]", Tokens["[NEW_REQ_OBJECT]"])
                    .Replace("()]", "]")
                    .Replace("(\"\")", "")
            ).ToArray();
        }
    }
}
