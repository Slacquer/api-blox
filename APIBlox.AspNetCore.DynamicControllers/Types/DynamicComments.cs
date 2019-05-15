using System;
using APIBlox.NetCore.Extensions;

namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class DynamicComments.
    /// </summary>
    public class DynamicComments
    {
        private const string RemarksTemplate = @"
/// <remarks>
/// @remarks
/// </remarks>
";

        private const string SummaryTemplate = @"
/// <summary>
/// @summary
/// </summary>
";

        /// <summary>
        ///     Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public string Summary { get; set; }

        /// <summary>
        ///     Gets or sets the remarks.
        /// </summary>
        /// <value>The remarks.</value>
        public string Remarks { get; set; }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            var summary = Summary.IsEmptyNullOrWhiteSpace()
                ? null
                : SummaryTemplate.Replace("@summary", Summary.Replace(Environment.NewLine, "\n/// "));
            var remarks = Remarks.IsEmptyNullOrWhiteSpace()
                ? null
                : RemarksTemplate.Replace("@remarks", Remarks.Replace(Environment.NewLine, "\n/// "));

            return $"{summary}{remarks}";
        }
    }
}
