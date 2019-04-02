using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using APIBlox.AspNetCore.Contracts;

namespace APIBlox.AspNetCore.Decorators.Commands
{
    /// <inheritdoc cref="ICommandHandler{TRequest}" />
    /// <summary>
    ///     Class TransactionScopeCommandHandlerDecorator.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t command.</typeparam>
    /// <seealso cref="T:APIBlox.AspNetCore.Decorators.CommandQueryDecoratorLoggingBase" />
    /// <seealso cref="T:APIBlox.AspNetCore.Contracts.ICommandHandler`1" />
    public class TransactionScopeCommandHandlerDecorator<TRequest>
        : ICommandHandler<TRequest>
    {
        private readonly ICommandHandler<TRequest> _decorated;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TransactionScopeCommandHandlerDecorator{TRequest}" /> class.
        /// </summary>
        /// <param name="decorated">The decorated.</param>
        public TransactionScopeCommandHandlerDecorator(ICommandHandler<TRequest> decorated)
        {
            _decorated = decorated;
        }

        /// <inheritdoc />
        public async Task HandleAsync(TRequest requestCommand, CancellationToken cancellationToken)
        {
            using (var scope = new TransactionScope())
            {
                await _decorated.HandleAsync(requestCommand, cancellationToken).ConfigureAwait(false);

                scope.Complete();
            }
        }
    }

    /// <inheritdoc cref="ICommandHandler{TRequest}" />
    /// <summary>
    ///     Class TransactionScopeCommandHandlerDecorator.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t command.</typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <seealso cref="T:APIBlox.AspNetCore.Decorators.CommandQueryDecoratorLoggingBase" />
    /// <seealso cref="T:APIBlox.AspNetCore.Contracts.ICommandHandler`1" />
    public class TransactionScopeCommandHandlerDecorator<TRequest, TResult>
        : ICommandHandler<TRequest, TResult>
    {
        private readonly ICommandHandler<TRequest, TResult> _decorated;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:APIBlox.AspNetCore.Decorators.Commands.TransactionScopeCommandHandlerDecorator`2" /> class.
        /// </summary>
        /// <param name="decorated">The decorated.</param>
        public TransactionScopeCommandHandlerDecorator(ICommandHandler<TRequest, TResult> decorated)
        {
            _decorated = decorated;
        }

        /// <inheritdoc />
        public async Task<TResult> HandleAsync(TRequest requestCommand, CancellationToken cancellationToken)
        {
            TResult ret;

            // https://particular.net/blog/transactionscope-and-async-await-be-one-with-the-flow
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ret = await _decorated.HandleAsync(requestCommand, cancellationToken).ConfigureAwait(false);

                scope.Complete();
            }

            return ret;
        }
    }
}
