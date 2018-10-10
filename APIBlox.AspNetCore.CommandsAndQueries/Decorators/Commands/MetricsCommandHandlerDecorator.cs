using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using Microsoft.Extensions.Logging;

namespace APIBlox.AspNetCore.Decorators.Commands
{
    /// <inheritdoc />
    /// <summary>
    ///     Class MetricsCommandHandlerDecorator.
    /// </summary>
    /// <typeparam name="TRequestCommand">The type of the t command.</typeparam>
    /// <seealso cref="T:APIBlox.AspNetCore.Decorators.CommandQueryDecoratorLoggingBase" />
    /// <seealso cref="T:APIBlox.AspNetCore.Contracts.ICommandHandler`1" />
    [DebuggerStepThrough]
    public class MetricsCommandHandlerDecorator<TRequestCommand>
        : ICommandHandler<TRequestCommand>
    {
        private readonly ICommandHandler<TRequestCommand> _decorated;
        private readonly ILogger<MetricsCommandHandlerDecorator<TRequestCommand>> _log;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MetricsCommandHandlerDecorator{TRequestCommand}" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger.</param>
        /// <param name="decorated">The decorated.</param>
        public MetricsCommandHandlerDecorator(
            ILoggerFactory loggerFactory,
            ICommandHandler<TRequestCommand> decorated
        )
        {
            _log = loggerFactory.CreateLogger<MetricsCommandHandlerDecorator<TRequestCommand>>();
            _decorated = decorated;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Handles the specified command.
        /// </summary>
        /// <param name="requestCommand">The incoming request command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task HandleAsync(TRequestCommand requestCommand, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            await _decorated.HandleAsync(requestCommand, cancellationToken).ConfigureAwait(false);

            sw.Stop();

            var elapsed = sw.Elapsed;

            if (elapsed.Seconds > 3)
                _log.LogCritical(() => $"!!!!!! VERY SLOW SPEED !!!!! Elapsed: {elapsed.ToString()}");
            else if (elapsed.Seconds > 2)
                _log.LogWarning(() => $"!!!!!! SLOW !!!!! Elapsed: {elapsed.ToString()}");
        }
    }

    /// <inheritdoc cref="ICommandHandler{TRequestCommand, TResult}" />
    /// <summary>
    ///     Class MetricsCommandHandlerDecorator.
    /// </summary>
    /// <typeparam name="TRequestCommand">The type of the t command.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <seealso cref="T:APIBlox.NetCore.LoggingBase" />
    /// <seealso cref="T:APIBlox.AspNetCore.Contracts.ICommandHandler`2" />
    [DebuggerStepThrough]
    public class MetricsCommandHandlerDecorator<TRequestCommand, TResult>
        : ICommandHandler<TRequestCommand, TResult>
    {
        private readonly ICommandHandler<TRequestCommand, TResult> _decorated;
        private readonly ILogger<MetricsCommandHandlerDecorator<TRequestCommand, TResult>> _log;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MetricsCommandHandlerDecorator{TRequestCommand, TResult}" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger.</param>
        /// <param name="decorated">The decorated.</param>
        public MetricsCommandHandlerDecorator(
            ILoggerFactory loggerFactory,
            ICommandHandler<TRequestCommand, TResult> decorated
        )
        {
            _log = loggerFactory.CreateLogger<MetricsCommandHandlerDecorator<TRequestCommand, TResult>>();
            _decorated = decorated;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Handles the specified command.
        /// </summary>
        /// <param name="requestCommand">The incoming request command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{TResult}.</returns>
        public async Task<TResult> HandleAsync(TRequestCommand requestCommand, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            var ret = await _decorated.HandleAsync(requestCommand, cancellationToken).ConfigureAwait(false);

            sw.Stop();

            var elapsed = sw.Elapsed;

            if (elapsed.Seconds > 3)
                _log.LogCritical(() => $"!!!!!! VERY SLOW SPEED !!!!! Elapsed: {elapsed.ToString()}");

            return ret;
        }
    }
}
