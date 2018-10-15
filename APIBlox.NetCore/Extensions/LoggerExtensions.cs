using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Logging
{
    /// <summary>
    ///     Class LoggerExtensions.
    /// </summary>

    //[DebuggerStepThrough]
    public static class LoggerExtensionsNetCore
    {
        /// <summary>
        ///     Logs critical only when logging is enabled for <see cref="LogLevel" /> Critical.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFunc">The message function.</param>
        public static void LogCritical(this ILogger logger, Func<string> messageFunc)
        {
            if (logger.IsEnabled(LogLevel.Critical))
                logger.LogCritical(messageFunc());
        }

        /// <summary>
        ///     Logs error only when logging is enabled for <see cref="LogLevel" />s Error and Critical.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFunc">The message function.</param>
        public static void LogError(this ILogger logger, Func<string> messageFunc)
        {
            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError(messageFunc());
        }

        /// <summary>
        ///     Logs information only when logging is enabled for <see cref="LogLevel" />s Information,Warning,Error and Critical.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFunc">The message function.</param>
        public static void LogInformation(this ILogger logger, Func<string> messageFunc)
        {
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation(messageFunc());
        }

        /// <summary>
        ///     Logs warning only when logging is enabled for <see cref="LogLevel" />s Warning,Error and Critical.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFunc">The message function.</param>
        public static void LogWarning(this ILogger logger, Func<string> messageFunc)
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning(messageFunc());
        }
    }
}
