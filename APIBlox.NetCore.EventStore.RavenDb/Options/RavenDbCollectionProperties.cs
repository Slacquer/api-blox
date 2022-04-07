using System;

namespace APIBlox.NetCore.Options
{
    /// <summary>
    ///     Class RavenDbCollectionProperties.
    /// </summary>
    public class RavenDbCollectionProperties
    {
        /// <summary>
        ///     Gets or sets the indexes.
        /// </summary>
        /// <value>The indexes.</value>
        public string[] Indexes { get; set; } = Array.Empty<string>();
    }
}
