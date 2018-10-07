using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;

namespace APIBlox.AspNetCore.Contracts
{
    /// <summary>
    ///     Interface IPatchCommandHandler, for patching that is designed to RETURN a result.  Returning anything from a
    ///     command is a violation of CQRS?  Don't like it, then use <see cref="IPatchCommandHandler{TRequestCommand}" />.
    /// </summary>
    /// <typeparam name="TRequestCommand">The type of the command.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IPatchCommandHandler<in TRequestCommand, TResult>
    {
        /// <summary>
        ///     Handles the specified command.
        /// </summary>
        /// <param name="requestCommand">The incoming request command.</param>
        /// <param name="patch">The patch operations document</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{TResult}.</returns>
        Task<TResult> HandleAsync(TRequestCommand requestCommand, JsonPatchDocument patch, CancellationToken cancellationToken);
    }


    /// <summary>
    ///     Interface IPatchCommandHandler, for patching that is designed to NOT RETURN a result.  Returning anything from a
    ///     command is a violation of CQRS, but if you need to, then use
    ///     <see cref="IPatchCommandHandler{TRequestCommand, TResult}" />.
    /// </summary>
    /// <typeparam name="TRequestCommand">The type of the command.</typeparam>
    public interface IPatchCommandHandler<in TRequestCommand>
    {
        /// <summary>
        ///     Handles the specified command.
        /// </summary>
        /// <param name="requestCommand">The incoming request command.</param>
        /// <param name="patch">The patch operations document</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task HandleAsync(TRequestCommand requestCommand, JsonPatchDocument patch, CancellationToken cancellationToken);
    }
}
