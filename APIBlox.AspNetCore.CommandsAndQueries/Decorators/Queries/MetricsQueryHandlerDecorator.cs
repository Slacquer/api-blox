using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using Microsoft.Extensions.Logging;

namespace APIBlox.AspNetCore.Decorators.Queries
{
    /// <inheritdoc cref="IQueryHandler{TResult}" />
    /// <summary>
    ///     Class MetricsQueryHandlerDecorator.
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <seealso cref="T:APIBlox.NetCore.LoggingBase" />
    /// <seealso cref="T:APIBlox.AspNetCore.Contracts.IQueryHandler`1" />
    [DebuggerStepThrough]
    public class MetricsQueryHandlerDecorator<TResult>
        : IQueryHandler<TResult>
    {
        private readonly IQueryHandler<TResult> _decorated;
        private readonly ILogger<MetricsQueryHandlerDecorator<TResult>> _log;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MetricsQueryHandlerDecorator{TResult}" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger.</param>
        /// <param name="decorated">The decorated.</param>
        public MetricsQueryHandlerDecorator(
            ILoggerFactory loggerFactory,
            IQueryHandler<TResult> decorated
        )
        {
            _log = loggerFactory.CreateLogger<MetricsQueryHandlerDecorator<TResult>>();
            _decorated = decorated;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Handles the specified query.
        /// </summary>
        public async Task<TResult> HandleAsync(CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            var ret = await _decorated.HandleAsync(cancellationToken).ConfigureAwait(false);

            sw.Stop();

            var elapsed = sw.Elapsed;

            if (elapsed.Seconds > 2)
                _log.LogCritical(() => $"!!!!!! VERY SLOW SPEED !!!!! Elapsed: {elapsed.ToString()}");

            return ret;
        }
    }

    /// <inheritdoc cref="IQueryHandler{TRequestQuery, TResult}" />
    /// <summary>
    ///     Class MetricsQueryHandlerDecorator.
    /// </summary>
    /// <typeparam name="TRequestQuery">The type of the t query.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <seealso cref="T:APIBlox.NetCore.LoggingBase" />
    /// <seealso cref="T:APIBlox.AspNetCore.Contracts.IQueryHandler`2" />
    [DebuggerStepThrough]
    public class MetricsQueryHandlerDecorator<TRequestQuery, TResult>
        : IQueryHandler<TRequestQuery, TResult>
    {
        private readonly IQueryHandler<TRequestQuery, TResult> _decorated;
        private readonly ILogger<MetricsQueryHandlerDecorator<TRequestQuery, TResult>> _log;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MetricsQueryHandlerDecorator{TRequestQuery, TResult}" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger.</param>
        /// <param name="decorated">The decorated.</param>
        public MetricsQueryHandlerDecorator(
            ILoggerFactory loggerFactory,
            IQueryHandler<TRequestQuery, TResult> decorated
        )
        {
            _log = loggerFactory.CreateLogger<MetricsQueryHandlerDecorator<TRequestQuery, TResult>>();
            _decorated = decorated;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Handles the specified query.
        /// </summary>
        public async Task<TResult> HandleAsync(
            TRequestQuery query,
            CancellationToken cancellationToken
        )
        {
            var sw = Stopwatch.StartNew();

            var ret = await _decorated.HandleAsync(query, cancellationToken).ConfigureAwait(false);

            sw.Stop();

            var elapsed = sw.Elapsed;

            if (elapsed.Seconds > 2)
                _log.LogCritical(() => $"!!!!!! VERY SLOW SPEED !!!!! Elapsed: {elapsed.ToString()}");

            return ret;
        }
    }
}
