﻿#region -    Using Statements    -

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using APIBlox.NetCore;
using APIBlox.NetCore.Extensions;

#endregion

namespace APIBlox.AspNetCore.Errors
{
    /// <inheritdoc />
    /// <summary>
    ///     Class ServerFaultResource.
    /// </summary>
    /// <seealso cref="DynamicErrorObject" />
    [Serializable]
    public class RequestErrorObject : DynamicDataObject
    {
        #region -    Constructors    -

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Errors.RequestErrorObject" /> class.
        /// </summary>
        public RequestErrorObject()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RequestErrorObject" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="detail">The detail.</param>
        /// <param name="status">The status.</param>
        /// <param name="instance">The instance.</param>
        /// <inheritdoc />
        public RequestErrorObject(string title, string detail, int status, string instance)
        {
            Title = title;
            Detail = detail;
            Status = status;
            Instance = instance;
        }

        #endregion

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
        ///         A URI reference that identifies the specific occurrence of the problem.It may or may not yield further
        ///         information.
        ///     </para>
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        ///     [REQUIRED]
        ///     <para>
        ///         The HTTP status code([RFC7231], Section 6) generated by the origin server for this occurrence of the problem.
        ///     </para>
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        ///     [REQUIRED]
        ///     <para>
        ///         A short, human-readable summary of the problem type.It SHOULD NOT change from occurrence to occurrence
        ///         of the problem, except for purposes of localization(e.g., using proactive content negotiation;
        ///         see[RFC7231], Section 3.4).
        ///     </para>
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     [REQUIRED] defaults to "about:blank"
        ///     <para>
        ///         A URI reference [RFC3986] that identifies the problem type. This specification encourages that, when
        ///         de-referenced, it provide human-readable documentation for the problem type
        ///         (e.g., using HTML [W3C.REC-html5-20141028]).  When this member is not present, its value is assumed to be
        ///         "about:blank".
        ///     </para>
        /// </summary>
        public string Type { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     <summary>
        ///         Returns the enumeration of all dynamic member names.
        ///         <para>
        ///             The following are validated and fail if empty:
        ///             <see cref="Status" />
        ///             <see cref="Instance" />
        ///         </para>
        ///         <para>
        ///             If empty, <see cref="Type" /> will be set to "about:blank"
        ///         </para>
        ///     </summary>
        /// </summary>
        /// <returns>A sequence that contains dynamic member names.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (Title.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException($"RFC7807 states that {GetType().Name}.{nameof(Title)} is required.", nameof(Title));

            if (Detail.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException($"RFC7807 states that {GetType().Name}.{nameof(Detail)} is required.", nameof(Detail));

            if (Type.IsEmptyNullOrWhiteSpace())
                Type = "about:blank";

            if (!Status.HasValue)
                throw new ArgumentException($"RFC7807 states that {GetType().Name}.{nameof(Status)} is required.", nameof(Status));

            if (Instance.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException($"RFC7807 states that {GetType().Name}.{nameof(Instance)} is required.", nameof(Instance));

            Properties.TryAdd("Type", Type);
            Properties.TryAdd("Title", Title);
            Properties.TryAdd("Detail", Detail);
            Properties.TryAdd("Status", Status);
            Properties.TryAdd("Instance", Instance);

            TryAlterRequestErrorObjectAction();

            if (Errors.Any())
                Properties.TryAdd("Errors", Errors);

            return base.GetDynamicMemberNames();
        }

        private void TryAlterRequestErrorObjectAction()
        {
            try
            {
                InternalHelpers.AlterRequestErrorObjectAction?.Invoke(this);
            }
            catch (Exception)
            {
                // Someone fucked up... do nothing.
            }
        }
    }
}
