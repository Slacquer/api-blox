using System;

namespace APIBlox.NetCore.Exceptions
{
    /// <summary>
    ///     Class EventStoreAccessException.
    ///     Implements the <see cref="System.Exception" />
    /// </summary>
    public class EventStoreAccessException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventStoreAccessException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EventStoreAccessException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventStoreAccessException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (
        ///     <see langword="Nothing" /> in Visual Basic) if no inner exception is specified.
        /// </param>
        public EventStoreAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
