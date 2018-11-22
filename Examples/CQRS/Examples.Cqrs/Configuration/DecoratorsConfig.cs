using Examples.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    internal static class DecoratorsConfig
    {
        public static IServiceCollection AddCqrsDecorators(this IServiceCollection services, ILoggerFactory loggerFactory)
        {
            services.AddCommandHandlerDecoration<SimplePostCommand>(loggerFactory, typeof(SimplePostCommandDecorator));

            return services;
        }
        
    }
}
