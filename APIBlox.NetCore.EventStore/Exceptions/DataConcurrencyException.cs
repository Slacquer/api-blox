﻿#region -    Using Statements    -

using System;

#endregion

namespace APIBlox.NetCore.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     Class DataConcurrencyException.
    /// </summary>
    public class DataConcurrencyException : Exception

    {
        #region -    Constructors    -

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.NetCore.Exceptions.DataConcurrencyException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DataConcurrencyException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.NetCore.Exceptions.DataConcurrencyException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (Nothing in
        ///     Visual Basic) if no inner exception is specified.
        /// </param>
        public DataConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}
