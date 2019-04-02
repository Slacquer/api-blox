﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Types;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace APIBlox.AspNetCore.Types
{
    /// <inheritdoc />
    /// <summary>
    ///     Class ServerFaultResource.
    /// </summary>
    [Serializable]
    public class RequestErrorObject : DynamicDataObject
    {
        /// <summary>
        ///     The logger
        /// </summary>
        [JsonIgnore]
        protected readonly ILogger<RequestErrorObject> Logger = new LoggerFactory().CreateLogger<RequestErrorObject>();

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Types.RequestErrorObject" /> class.
        /// </summary>
        public RequestErrorObject()
        : this(null, null, null, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestErrorObject"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="detail">The detail.</param>
        /// <param name="status">The status.</param>
        /// <param name="instance">The instance.</param>
        public RequestErrorObject(string title, string detail, int? status, string instance)
        {
            Title = title;
            Detail = detail;
            Status = status;
            Instance = instance;
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
        ///         A URI reference that identifies the specific occurrence of the problem.It may or may not yield further
        ///         information.
        ///     </para>
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [no throw].
        /// </summary>
        /// <value><c>true</c> if [no throw]; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        internal bool NoThrow { get; set; }

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
            {
                var msg = $"RFC7807 states that {GetType().Name}" +
                          $".{nameof(Title)} is required.";

                if (NoThrow)
                    Logger.LogError(() => msg);
                else
                    throw new ArgumentException(msg, nameof(Title));
            }
            else
                Properties.TryAdd("Title", Title);

            if (Detail.IsEmptyNullOrWhiteSpace())
            {
                var msg = $"RFC7807 states that {GetType().Name}" +
                          $".{nameof(Detail)} is required.";

                if (NoThrow)
                    Logger.LogError(() => msg);
                else
                    throw new ArgumentException(msg, nameof(Detail));
            }
            else
                Properties.TryAdd("Detail", Detail);

            if (Type.IsEmptyNullOrWhiteSpace())
                Type = "about:blank";

            Properties.TryAdd("Type", Type);

            if (!Status.HasValue)
            {
                var msg = $"RFC7807 states that {GetType().Name}" +
                          $".{nameof(Status)} is required.";

                if (NoThrow)
                    Logger.LogError(() => msg);
                else
                    throw new ArgumentException(msg, nameof(Status));
            }
            else
                Properties.TryAdd("Status", Status);

            if (Instance.IsEmptyNullOrWhiteSpace())
            {
                var msg = $"RFC7807 states that {GetType().Name}" +
                          $".{nameof(Instance)} is required.";

                if (NoThrow)
                    Logger.LogError(() => msg);
                else
                    throw new ArgumentException(msg, nameof(Instance));
            }
            else
                Properties.TryAdd("Instance", Instance);

            TryAlterRequestObjectAction();

            if (!(Errors is null) && Errors.Any())
                Properties.TryAdd("Errors", Errors);

            return base.GetDynamicMemberNames();
        }

        private void TryAlterRequestObjectAction()
        {
            try
            {
                InternalHelpers.AlterRequestErrorObjectAction?.Invoke(this);
            }
            catch (Exception ex)
            {
                Logger.LogError(() =>
                    $"An error has occured while invoking AddAlterRequestErrorObject.alterAction.  Ex: {ex.Message}"
                );
            }
        }
    }
}
