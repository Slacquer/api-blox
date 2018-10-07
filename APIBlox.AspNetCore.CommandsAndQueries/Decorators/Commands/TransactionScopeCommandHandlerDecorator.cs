#region -    Using Statements    -

using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using APIBlox.AspNetCore.Contracts;

#endregion

namespace APIBlox.AspNetCore.Decorators.Commands
{
    /// <inheritdoc cref="ICommandHandler{TRequestCommand}" />
    /// <summary>
    ///     Class TransactionScopeCommandHandlerDecorator.
    /// </summary>
    /// <typeparam name="TRequestCommand">The type of the t command.</typeparam>
    /// <seealso cref="T:APIBlox.AspNetCore.Decorators.CommandQueryDecoratorLoggingBase" />
    /// <seealso cref="T:APIBlox.AspNetCore.Contracts.ICommandHandler`1" />
    public class TransactionScopeCommandHandlerDecorator<TRequestCommand>
        : ICommandHandler<TRequestCommand>
    {
        #region -    Fields    -

        private readonly ICommandHandler<TRequestCommand> _decorated;

        #endregion

        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="TransactionScopeCommandHandlerDecorator{TRequestCommand}" /> class.
        /// </summary>
        /// <param name="decorated">The decorated.</param>
        public TransactionScopeCommandHandlerDecorator(ICommandHandler<TRequestCommand> decorated)
        {
            _decorated = decorated;
        }

        #endregion

        /// <inheritdoc />
        public async Task HandleAsync(TRequestCommand requestCommand, CancellationToken cancellationToken)
        {
            using (var scope = new TransactionScope())
            {
                await _decorated.HandleAsync(requestCommand, cancellationToken).ConfigureAwait(false);
                scope.Complete();
            }
        }
    }

    /// <inheritdoc cref="ICommandHandler{TRequestCommand}" />
    /// <summary>
    ///     Class TransactionScopeCommandHandlerDecorator.
    /// </summary>
    /// <typeparam name="TRequestCommand">The type of the t command.</typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <seealso cref="T:APIBlox.AspNetCore.Decorators.CommandQueryDecoratorLoggingBase" />
    /// <seealso cref="T:APIBlox.AspNetCore.Contracts.ICommandHandler`1" />
    public class TransactionScopeCommandHandlerDecorator<TRequestCommand, TResult>
        : ICommandHandler<TRequestCommand, TResult>
    {
        #region -    Fields    -

        private readonly ICommandHandler<TRequestCommand, TResult> _decorated;

        #endregion

        #region -    Constructors    -

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:APIBlox.AspNetCore.Decorators.Commands.TransactionScopeCommandHandlerDecorator`2" /> class.
        /// </summary>
        /// <param name="decorated">The decorated.</param>
        public TransactionScopeCommandHandlerDecorator(ICommandHandler<TRequestCommand, TResult> decorated)
        {
            _decorated = decorated;
        }

        #endregion

        /// <inheritdoc />
        public async Task<TResult> HandleAsync(TRequestCommand requestCommand, CancellationToken cancellationToken)
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
