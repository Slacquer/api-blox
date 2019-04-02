using System;
using APIBlox.AspNetCore.Types;

namespace APIBlox.AspNetCore.Exceptions
{
    /// <summary>
    ///     Class HandledRequestException.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class HandledRequestException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HandledRequestException" /> class.
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
