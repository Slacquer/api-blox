using APIBlox.NetCore.Extensions;
//using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    internal static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerExampleFeatures(this IServiceCollection services, string siteTitle, string version,
            string dynamicControllersXmlFile
        )
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
                    c.IncludeXmlComments(@".\Examples.DynamicControllers.xml", true);

                    if (!dynamicControllersXmlFile.IsEmptyNullOrWhiteSpace())
                        c.IncludeXmlComments(dynamicControllersXmlFile, true);

                    //c.AddFluentValidationRules();
                    //c.SchemaFilter<FluentValidationRules>();
                    //c.OperationFilter<FluentValidationOperationFilter>();
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
