﻿namespace APIBlox.AspNetCore.Services
{
    /// <summary>
    ///     Class PaginationMetadata.
    /// </summary>
    public class PaginationMetadata
    {
        /// <summary>
        ///     Gets or sets the count for this result.
        /// </summary>
        /// <value>The count.</value>
        public long? ResultCount { get; set; }

        /// <summary>
        ///     Gets or sets the next.
        /// </summary>
        /// <value>The next.</value>
        public string Next { get; set; }

        /// <summary>
        ///     Gets or sets the previous.
        /// </summary>
        /// <value>The previous.</value>
        public string Previous { get; set; }
    }
}
