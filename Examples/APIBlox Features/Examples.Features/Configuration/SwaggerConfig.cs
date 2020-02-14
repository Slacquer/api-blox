using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    internal static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerExampleFeatures(this IServiceCollection services, string siteTitle, string version)
        {
            return services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(version,
                        new OpenApiInfo
                        {
                            Title = siteTitle,
                            Version = version
                        }
                    );
                    c.IncludeXmlComments(@".\Examples.Features.xml", true);
                }
            );
        }

        public static IApplicationBuilder UseSwaggerExampleFeatures(this IApplicationBuilder app, string siteTitle, string version)
        {
            return app.UseSwagger()
                .UseSwaggerUI(s =>
                    s.SwaggerEndpoint($"/swagger/{version}/swagger.json",
                        $"{siteTitle} {version}"
                    )
                );
        }
    }
}
