#region -    Using Statements    -

using System;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using APIBlox.AspNetCore.Errors;
using APIBlox.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace APIBlox.AspNetCore.Filters.Authorization_Filters
{
    /// <inheritdoc />
    /// <summary>
    ///     Class ProblemResultAuthorizationFilter.
    ///     <para>
    ///         Will allow anonymous access when the  <see cref="T:Microsoft.AspNetCore.Mvc.Authorization.IAllowAnonymousFilter" /> is used.  Otherwise sends
    ///         <see cref="T:APIBlox.AspNetCore.ActionResults.ProblemResult" />
    ///     </para>
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.Filters.IAsyncAuthorizationFilter" />
    public class ProblemResultAuthorizationFilter : IAsyncAuthorizationFilter
    {
        #region -    Constructors    -

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProblemResultAuthorizationFilter" /> class.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <exception cref="ArgumentNullException">policy</exception>
        public ProblemResultAuthorizationFilter(AuthorizationPolicy policy)
        {
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        #endregion

        /// <summary>
        ///     Gets the policy that was added during construction.
        /// </summary>
        /// <value>The policy.</value>
        public AuthorizationPolicy Policy { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Authorization as an asynchronous operation.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext" />.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that on completion indicates the filter has executed.</returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Allow Anonymous skips all authorization
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
                return;

            var policyEvaluator = context.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();

            var authenticateResult = await policyEvaluator.AuthenticateAsync(Policy, context.HttpContext);
            var authorizeResult = await policyEvaluator.AuthorizeAsync(Policy, authenticateResult, context.HttpContext, context);

            var errObject = new RequestErrorObject();

            if (authorizeResult.Challenged)
                errObject.SetErrorToUnAuthorized();
            else if (authorizeResult.Forbidden)
                errObject.SetErrorToForbidden(string.Join(", ", Policy.AuthenticationSchemes));

            context.Result = new ProblemResult(errObject);
        }
    }
}
