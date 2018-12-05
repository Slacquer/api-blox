using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;

namespace APIBlox.AspNetCore.Contracts
{
    /// <summary>
    ///     Interface IPatchCommandHandler, for patching that is designed to RETURN a result.  Returning anything from a
    ///     command is a violation of CQRS?  Don't like it, then use <see cref="IPatchCommandHandler{TRequestCommand}" />.
    /// </summary>
    /// <typeparam name="TPatchRequestCommand">The type of the command.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <typeparam name="TPatchObject">Type of patch object</typeparam>
    public interface IGenericPatchCommandHandler<in TPatchRequestCommand, TPatchObject, TResult>
        where TPatchObject : class
    {
        /// <summary>
        ///     Handles the specified command.
        /// </summary>
        /// <param name="requestCommand">The incoming request command.</param>
        /// <param name="patchDocument">The incoming patch document.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{TResult}.</returns>
        Task<TResult> HandleAsync(TPatchRequestCommand requestCommand, JsonPatchDocument<TPatchObject> patchDocument,
            CancellationToken cancellationToken
        );
    }

    /// <summary>
    ///     Interface IPatchCommandHandler, for patching that is designed to RETURN a result.  Returning anything from a
    ///     command is a violation of CQRS?  Don't like it, then use <see cref="IPatchCommandHandler{TRequestCommand}" />.
    /// </summary>
    /// <typeparam name="TPatchRequestCommand">The type of the command.</typeparam>
    /// <typeparam name="TPatchObject">Type of patch object</typeparam>
    public interface IGenericPatchCommandHandler<in TPatchRequestCommand, TPatchObject>
        where TPatchObject : class
    {
        /// <summary>
        ///     Handles the specified command.
        /// </summary>
        /// <param name="requestCommand">The incoming request command.</param>
        /// <param name="patchDocument">The incoming patch document.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{TResult}.</returns>
        Task HandleAsync(TPatchRequestCommand requestCommand, JsonPatchDocument<TPatchObject> patchDocument, CancellationToken cancellationToken);
    }
}
