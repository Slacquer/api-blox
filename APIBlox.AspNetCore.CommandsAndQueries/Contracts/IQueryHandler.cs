using System.Threading;
using System.Threading.Tasks;

namespace APIBlox.AspNetCore.Contracts
{
    /// <summary>
    ///     Interface IQueryHandler, used when your QUERY requires input parameters, IE: GetSomething(requirements)
    /// </summary>
    /// <typeparam name="TRequestQuery">The type of the query.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IQueryHandler<in TRequestQuery, TResult>
    {
        /// <summary>
        ///     Handles the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{TResult}.</returns>
        Task<TResult> HandleAsync(TRequestQuery query, CancellationToken cancellationToken);
    }

    /// <summary>
    ///     Interface IQueryHandler, used when your QUERY does not require input parameters, IE: GetAll()
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IQueryHandler<TResult>
    {
        /// <summary>
        ///     Handles the specified cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{TResult}.</returns>
        Task<TResult> HandleAsync(CancellationToken cancellationToken);
    }
}
