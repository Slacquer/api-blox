using System.Linq.Expressions;

namespace APIBlox.NetCore.Extensions
{
    /// <summary>
    ///     Class ExpressionExtensions.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        ///     Simplified AndAlso expression generation.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the t delegate.</typeparam>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Expression&lt;TDelegate&gt;.</returns>
        public static Expression<TDelegate> AndAlso<TDelegate>(this Expression<TDelegate> left, Expression<TDelegate> right)
        {
            return Expression.Lambda<TDelegate>(Expression.AndAlso(left, right), left.Parameters);
        }
    }
}
