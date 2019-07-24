using APIBlox.NetCore.Decorators.Commands;
using APIBlox.NetCore.Extensions;
using Examples.Commands;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    internal static class DecoratorsConfig
    {
        public static IServiceCollection AddCqrsDecorators(this IServiceCollection services, ILoggerFactory loggerFactory)
        {
            services.AddCommandHandlerDecoration<SimplePostCommand>(loggerFactory,

                //
                // Nothing more than a stopwatch.  Ideally (my opinion) an API
                // call should never take more than 3 seconds.
                typeof(MetricsCommandHandlerDecorator<,>),
                
                //
                // This one is still a work in progress and is most likely
                // more valuable when combined with domain events.
                typeof(TransactionScopeCommandHandlerDecorator<,>)
                , typeof(SimplePostCommandDecorator)
            );

            return services;
        }
    }
}
