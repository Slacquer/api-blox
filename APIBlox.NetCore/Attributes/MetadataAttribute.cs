#region -    Using Statements    -

using System;

#endregion

namespace APIBlox.NetCore.Attributes
{
    /// <inheritdoc />
    /// <summary>
    ///     Class MetadataAttribute.
    ///     <para>
    ///         Lame but helpful.
    ///     </para>
    /// </summary>
    /// <seealso cref="T:System.Attribute" />
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MetadataAttribute : Attribute
    {
        #region -    Constructors    -

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.NetCore.Attributes.MetadataAttribute" /> class.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="v3">The v3.</param>
        /// <param name="v4">The v4.</param>
        /// <param name="v5">The v5.</param>
        /// <param name="v6">The v6.</param>
        /// <param name="v7">The v7.</param>
        /// <param name="v8">The v8.</param>
        /// <param name="v9">The v9.</param>
        /// <param name="v10">The V10.</param>
        public MetadataAttribute(
            object v1 = null, object v2 = null, object v3 = null,
            object v4 = null, object v5 = null, object v6 = null,
            object v7 = null, object v8 = null, object v9 = null, object v10 = null
        )
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
            V5 = v5;
            V6 = v6;
            V7 = v7;
            V8 = v8;
            V9 = v9;
            V10 = v10;
        }

        #endregion

        /// <summary>
        ///     Gets the v1.
        /// </summary>
        /// <value>The v1.</value>
        public object V1 { get; }

        /// <summary>
        ///     Gets the V10.
        /// </summary>
        /// <value>The V10.</value>
        public object V10 { get; }

        /// <summary>
        ///     Gets the v2.
        /// </summary>
        /// <value>The v2.</value>
        public object V2 { get; }

        /// <summary>
        ///     Gets the v3.
        /// </summary>
        /// <value>The v3.</value>
        public object V3 { get; }

        /// <summary>
        ///     Gets the v4.
        /// </summary>
        /// <value>The v4.</value>
        public object V4 { get; }

        /// <summary>
        ///     Gets the v5.
        /// </summary>
        /// <value>The v5.</value>
        public object V5 { get; }

        /// <summary>
        ///     Gets the v6.
        /// </summary>
        /// <value>The v6.</value>
        public object V6 { get; }

        /// <summary>
        ///     Gets the v7.
        /// </summary>
        /// <value>The v7.</value>
        public object V7 { get; }

        /// <summary>
        ///     Gets the v8.
        /// </summary>
        /// <value>The v8.</value>
        public object V8 { get; }

        /// <summary>
        ///     Gets the v9.
        /// </summary>
        /// <value>The v9.</value>
        public object V9 { get; }
    }
}
