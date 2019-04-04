﻿using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.Swagger;

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
                    c.DescribeAllEnumsAsStrings();
                    c.SwaggerDoc(version, new Info {Title = siteTitle, Version = version});
                    c.IncludeXmlComments(@".\Examples.DynamicControllers.xml", true);

                    if (!dynamicControllersXmlFile.IsEmptyNullOrWhiteSpace())
                        c.IncludeXmlComments(dynamicControllersXmlFile, true);

                    c.AddFluentValidationRules();
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
