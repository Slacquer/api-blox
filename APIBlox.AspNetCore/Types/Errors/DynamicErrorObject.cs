using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using APIBlox.NetCore;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;

namespace APIBlox.AspNetCore.Types.Errors
{
    /// <inheritdoc />
    /// <summary>
    ///     Class DynamicErrorObject.
    /// </summary>
    /// <seealso cref="T:System.Dynamic.DynamicObject" />
    public class DynamicErrorObject : DynamicDataObject
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicErrorObject" /> class.
        /// </summary>
        public DynamicErrorObject()
            : this(null, null)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.Errors.DynamicErrorObject" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="detail">The detail.</param>
        public DynamicErrorObject(string title, string detail)
        {
            Title = title;
            Detail = detail;
        }

        /// <summary>
        ///     [REQUIRED]
        ///     <para>
        ///         Required when <see cref="Errors" /> is not empty.
        ///     </para>
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        ///     Gets or sets the inner errors.
        /// </summary>
        /// <remarks>
        ///     An object containing more specific information.
        /// </remarks>
        /// <value>The inner error.</value>
        public Collection<DynamicErrorObject> Errors { get; set; } = new Collection<DynamicErrorObject>();

        /// <summary>
        ///     [REQUIRED]
        ///     <para>
        ///         A short, human-readable summary of the problem type.It SHOULD NOT change from occurrence to occurrence
        ///         of the problem, except for purposes of localization(e.g., using proactive content negotiation;
        ///         see[RFC7231], Section 3.4).
        ///     </para>
        /// </summary>
        public string Title { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Returns the one and only Error property.
        ///     <para>
        ///         <see cref="Title" /> will be validated and fail if empty.
        ///     </para>
        ///     <para>
        ///         <see cref="Detail" /> will be validated and fail if empty when <see cref="Errors" /> is not empty.
        ///     </para>
        /// </summary>
        /// <returns>A sequence that contains dynamic member names.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var errors = Errors?.Any() == true;

            if (Detail.IsEmptyNullOrWhiteSpace() && errors)
                throw new ArgumentException(
                    "Although not required deeper than the root, we will require" +
                    $" {GetType().Name}.{nameof(Detail)} when errors is not empty.",
                    nameof(Detail)
                );

            if (Title.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException(
                    "Although not required deeper than the root, we will require " +
                    $"{GetType().Name}.{nameof(Title)}",
                    nameof(Title)
                );

            Properties.TryAdd("Title", Title);
            Properties.TryAdd("Detail", Detail);

            if (errors)
                Properties.Add("Errors", Errors);

            return base.GetDynamicMemberNames();
        }
    }
}
