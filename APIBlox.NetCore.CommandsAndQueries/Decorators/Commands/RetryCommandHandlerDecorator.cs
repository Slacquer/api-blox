using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Contracts;

namespace APIBlox.NetCore.Decorators.Commands
{
    /// <summary>
    ///     Class RetryCommandHandlerDecorator.  Simply retries twice, with a 1 second delay.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <seealso cref="APIBlox.NetCore.Contracts.ICommandHandler{TRequest, TResult}" />
    public class RetryCommandHandlerDecorator<TRequest, TResult>
        : ICommandHandler<TRequest, TResult>
    {
        private readonly ICommandHandler<TRequest, TResult> _decorated;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RetryCommandHandlerDecorator{TRequest, TResult}" /> class.
        /// </summary>
        /// <param name="decorated">The decorated command.</param>
        public RetryCommandHandlerDecorator(ICommandHandler<TRequest, TResult> decorated)
        {
            _decorated = decorated;
        }

        /// <summary>
        ///     Handle as an asynchronous operation.
        /// </summary>
        /// <param name="requestCommand">The incoming request command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{TResult}.</returns>
        public async Task<TResult> HandleAsync(TRequest requestCommand, CancellationToken cancellationToken)
        {
            TResult ret = default;

            for (var i = 0; i < 3; i++)
                try
                {
                    ret = await _decorated.HandleAsync(requestCommand, cancellationToken).ConfigureAwait(false);

                    break;
                }
                catch
                {
                    if (i == 2)
                        throw;

                    await Task.Delay(1000, cancellationToken);
                }

            return ret;
        }
    }

    /// <summary>
    ///     Class RetryCommandHandlerDecorator.  Simple retries twice, with a 1 second delay.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <seealso cref="APIBlox.NetCore.Contracts.ICommandHandler{TRequest, TResult}" />
    public class RetryCommandHandlerDecorator<TRequest>
        : ICommandHandler<TRequest>
    {
        private readonly ICommandHandler<TRequest> _decorated;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RetryCommandHandlerDecorator{TRequest, TResult}" /> class.
        /// </summary>
        /// <param name="decorated">The decorated command.</param>
        public RetryCommandHandlerDecorator(ICommandHandler<TRequest> decorated)
        {
            _decorated = decorated;
        }

        /// <summary>
        ///     Handle as an asynchronous operation.
        /// </summary>
        /// <param name="requestCommand">The incoming request command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{TResult}.</returns>
        public async Task HandleAsync(TRequest requestCommand, CancellationToken cancellationToken)
        {
            for (var i = 0; i < 3; i++)
                try
                {
                    await _decorated.HandleAsync(requestCommand, cancellationToken).ConfigureAwait(false);

                    break;
                }
                catch
                {
                    if (i == 2)
                        throw;

                    await Task.Delay(1000, cancellationToken);
                }
        }
    }
}
