using System;

namespace APIBlox.NetCore.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     Class DataAccessException.
    /// </summary>
    /// <seealso cref="T:System.Exception" />
    public class DataAccessException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.NetCore.Exceptions.DataAccessException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DataAccessException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.NetCore.Exceptions.DataAccessException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (Nothing in
        ///     Visual Basic) if no inner exception is specified.
        /// </param>
        public DataAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
