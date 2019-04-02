using System;
using APIBlox.AspNetCore.Types;

namespace APIBlox.AspNetCore.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     Class HandledRequestException.
    /// </summary>
    /// <seealso cref="T:System.Exception" />
    public class HandledRequestException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:APIBlox.AspNetCore.Exceptions.HandledRequestException" /> class.
        /// </summary>
        /// <param name="requestErrorObject">The request error object.</param>
        public HandledRequestException(RequestErrorObject requestErrorObject)
        {
            RequestErrorObject = requestErrorObject;
        }

        /// <summary>
        ///     Gets the request error object.
        /// </summary>
        /// <value>The request error object.</value>
        public RequestErrorObject RequestErrorObject { get; }
    }
}
