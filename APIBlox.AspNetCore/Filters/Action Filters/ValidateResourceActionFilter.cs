#region -    Using Statements    -

using System.Threading.Tasks;
using APIBlox.AspNetCore.ActionResults;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

#endregion

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore.Filters
{
    internal class ValidateResourceActionFilter : IAsyncActionFilter
    {
        #region -    Fields    -

        private readonly ILogger<ValidateResourceActionFilter> _log;

        #endregion

        #region -    Constructors    -

        public ValidateResourceActionFilter(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<ValidateResourceActionFilter>();
        }

        #endregion

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid)
            {
                _log.LogInformation(() => $"Validation succeeded for {context.Result}");

                await next().ConfigureAwait(false);
                
                return;
            }

            _log.LogWarning(() => $"Validation failed for {context.Result}, returning new ValidationFailureResult");

            context.Result = new ValidationFailureResult();
        }
    }
}
