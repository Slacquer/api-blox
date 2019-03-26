namespace APIBlox.NetCore.Options
{
    /// <summary>
    ///     Class EfCoreSqlOptions.
    /// </summary>
    public class EfCoreSqlOptions
    {
        /// <summary>
        ///     Gets or sets the CNN string.
        /// </summary>
        /// <value>The CNN string.</value>
        public string CnnString { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [enable detailed errors].
        /// </summary>
        /// <value><c>true</c> if [enable detailed errors]; otherwise, <c>false</c>.</value>
        public bool EnableDetailedErrors { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [enable sensitive data logging].
        /// </summary>
        /// <value><c>true</c> if [enable sensitive data logging]; otherwise, <c>false</c>.</value>
        public bool EnableSensitiveDataLogging { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [configure warnings].
        /// </summary>
        /// <value><c>true</c> if [configure warnings]; otherwise, <c>false</c>.</value>
        public bool ConfigureWarnings { get; set; }
    }
}
