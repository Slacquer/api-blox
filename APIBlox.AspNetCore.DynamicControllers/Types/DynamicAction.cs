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
        /// <param name="ctorArgs">The ctor arguments.</param>
        /// <param name="ctorBody">The ctor body.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="namespaces">The namespaces.</param>
        /// <param name="methods">The methods.</param>
        public DynamicAction(string name, string route, string content, string ctorArgs, string ctorBody, string[] fields, string[] namespaces, string methods = null)
        {
            Name = name;
            Route = route;
            Content = content;
            CtorArgs = ctorArgs;
            CtorBody= ctorBody;
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
        public string CtorArgs { get; private set; }

        /// <summary>
        /// Gets the ctor body.
        /// </summary>
        /// <value>The ctor body.</value>
        public string CtorBody { get; private set; }

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
        public string Methods { get; private set; }

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
            Content = ParseTokens(Content);
            CtorArgs = ParseTokens(CtorArgs);
            CtorBody = ParseTokens(CtorBody);
            Fields = Fields.Select(ParseTokens).ToArray();

            if (Methods is null || !Methods.Any())
                return;

            Methods = ParseTokens(Methods);
        }

        private string ParseTokens(string input)
        {
            return input
                .Replace("[REQ_OBJECT]", Tokens["[REQ_OBJECT]"])
                .Replace("[RES_OBJECT_INNER_RESULT]", Tokens["[RES_OBJECT_INNER_RESULT]"])
                .Replace("[ACTION_ROUTE]", Tokens["[ACTION_ROUTE]"])
                .Replace("[PARAMS_COMMENTS]", Tokens["[PARAMS_COMMENTS]"])
                .Replace("[RES_OBJECT_RESULT]", Tokens["[RES_OBJECT_RESULT]"])
                .Replace("[ACTION_PARAMS]", Tokens["[ACTION_PARAMS]"])
                .Replace("[NEW_REQ_OBJECT]", Tokens["[NEW_REQ_OBJECT]"])
                .Replace("[CONTROLLER_NAME]", Tokens["[CONTROLLER_NAME]"])

                .Replace("()]", "]")
                .Replace("(\"\")", "");
        }
    }
}
