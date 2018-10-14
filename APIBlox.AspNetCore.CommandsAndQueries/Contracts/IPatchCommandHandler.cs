using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Types;

namespace APIBlox.AspNetCore.Contracts
{
    /// <summary>
    ///     Interface IPatchCommandHandler, for patching that is designed to RETURN a result.  Returning anything from a
    ///     command is a violation of CQRS?  Don't like it, then use <see cref="IPatchCommandHandler{TRequestCommand}" />.
    /// </summary>
    /// <typeparam name="TPatchRequestCommand">The type of the command.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IPatchCommandHandler<in TPatchRequestCommand, TResult>
        where TPatchRequestCommand : PatchRequest
    {
        /// <summary>
        ///     Handles the specified command.
        /// </summary>
        /// <param name="requestCommand">The incoming request command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{TResult}.</returns>
        Task<TResult> HandleAsync(TPatchRequestCommand requestCommand, CancellationToken cancellationToken);
    }

    /// <summary>
    ///     Interface IPatchCommandHandler, for patching that is designed to NOT RETURN a result.  Returning anything from a
    ///     command is a violation of CQRS, but if you need to, then use
    ///     <see cref="IPatchCommandHandler{TRequestCommand, TResult}" />.
    /// </summary>
    /// <typeparam name="TPatchRequestCommand">The type of the command.</typeparam>
    public interface IPatchCommandHandler<in TPatchRequestCommand>
        where TPatchRequestCommand : PatchRequest
    {
        /// <summary>
        ///     Handles the specified command.
        /// </summary>
        /// <param name="requestCommand">The incoming request command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task HandleAsync(TPatchRequestCommand requestCommand, CancellationToken cancellationToken);
    }

    /// <summary>
    ///     Interface IPatchCommandHandler, for patching that is designed to RETURN a result.  Returning anything from a
    ///     command is a violation of CQRS?  Don't like it, then use <see cref="IPatchCommandHandler{TRequestCommand}" />.
    /// </summary>
    /// <typeparam name="TPatchRequestCommand">The type of the command.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <typeparam name="TPatchObject">The patch object</typeparam>
    public interface IPatchCommandHandler<in TPatchRequestCommand, TPatchObject, TResult>
        where TPatchRequestCommand : PatchRequest<TPatchObject>
        where TPatchObject : class
    {
        /// <summary>
        ///     Handles the specified command.
        /// </summary>
        /// <param name="requestCommand">The incoming request command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{TResult}.</returns>
        Task<TResult> HandleAsync(TPatchRequestCommand requestCommand, CancellationToken cancellationToken);
    }
}
