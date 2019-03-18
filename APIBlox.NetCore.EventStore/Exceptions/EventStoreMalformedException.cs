using System;

namespace APIBlox.NetCore.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     Class EventStoreMalformedException.
    ///     Implements the <see cref="System.Exception" />
    /// </summary>
    public class EventStoreMalformedException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventStoreMalformedException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EventStoreMalformedException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventStoreMalformedException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (Nothing in
        ///     Visual Basic) if no inner exception is specified.
        /// </param>
        public EventStoreMalformedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
