#region -    Using Statements    -

using System;
using System.Collections.Generic;
using System.Linq;
using APIBlox.NetCore.Extensions;

#endregion

namespace APIBlox.AspNetCore.Errors
{
    /// <inheritdoc />
    /// <summary>
    ///     Class ServerError.
    /// </summary>
    /// <seealso cref="RequestErrorObject" />
    public class ServerErrorObject : RequestErrorObject
    {
        #region -    Constructors    -

        /// <inheritdoc />
        public ServerErrorObject(string title, string detail, int status, string instance, string referenceId)
            : base(title, detail, status, instance)
        {
            ReferenceId = referenceId;
        }

        #endregion

        /// <summary>
        ///     [REQUIRED]
        ///     <para>
        ///         Gets or sets the reference id.
        ///     </para>
        /// </summary>
        /// <value>The reference id.</value>
        public string ReferenceId { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Returns the enumeration of all dynamic member names.
        ///     <para>
        ///         The following are validated and fail if empty:
        ///         <see cref="ReferenceId" />
        ///     </para>
        /// </summary>
        /// <returns>A sequence that contains dynamic member names.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (ReferenceId.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException(
                    $"Although {GetType().Name}.{nameof(ReferenceId)} is not required by RFC7807, we still want it!",
                    nameof(ReferenceId)
                );

            Properties.TryAdd("ReferenceId", ReferenceId);

            if (Errors.Any())
                Properties.TryAdd("Errors", Errors);

            return base.GetDynamicMemberNames();
        }
    }
}
