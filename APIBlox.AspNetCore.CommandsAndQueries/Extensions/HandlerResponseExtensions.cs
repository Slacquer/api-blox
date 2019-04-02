using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore.Types
{
    /// <summary>
    ///     Class HandlerResponseExtensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class HandlerResponseExtensions
    {
        /// <summary>
        ///     Factory method to create a new <see cref="RequestErrorObject" />
        ///     for the <see cref="HandlerResponse" /> instance.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>HandlerResponse.</returns>
        public static RequestErrorObject CreateError(this HandlerResponse response)
        {
            response.Error = new RequestErrorObject();

            return response.Error;
        }
    }
}
