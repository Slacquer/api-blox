using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;
using Newtonsoft.Json;

namespace APIBlox.AspNetCore.Types
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
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.DynamicErrorObject" /> class.
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
        ///     Gets or sets a value indicating whether [no throw].
        /// </summary>
        /// <value><c>true</c> if [no throw]; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        internal bool NoThrow { get; set; }

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
            {
                var msg = "Although not required deeper than the root, we will require" +
                          $" {GetType().Name}.{nameof(Detail)} when errors is not empty.";

                if (!NoThrow)
                    throw new ArgumentException(msg, nameof(Detail));
            }
            else
                Properties.TryAdd("Detail", Detail);

            if (Title.IsEmptyNullOrWhiteSpace())
            {
                var msg = "Although not required deeper than the root, we will require " +
                          $"{GetType().Name}.{nameof(Title)}";

                if (!NoThrow)
                    throw new ArgumentException(msg, nameof(Title));
            }
            else
                Properties.TryAdd("Title", Title);

            if (errors)
                Properties.TryAdd("Errors", Errors);

            return base.GetDynamicMemberNames();
        }
    }
}
