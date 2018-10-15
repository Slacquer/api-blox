using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.OpenApi.Models;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensionsOpenApi
    {
        public static IServiceCollection AddOpenApiBits(this IServiceCollection services)
        {
            var doc = new Microsoft.OpenApi.Models.OpenApiDocument();
            doc.Components = new OpenApi.Models.OpenApiComponents();
            doc.Components.Schemas = new Dictionary<string, OpenApiSchema>();


            doc.Info = new OpenApiInfo
            {
                Title = "Required title",
                Version = "required version 1"
            };

            doc.Paths = new OpenApiPaths
            {
                {
                    "/actualRoutePath/{id}", new OpenApiPathItem
                    {
                        Description = "root path description",
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Get,
                                new OpenApiOperation {Description = "get op description"}
                            }
                        },
                        Parameters=new List<OpenApiParameter>{new OpenApiParameter{ In= ParameterLocation.Path, Name="id"}}
                    }
                }
            };

            //doc.Components.Schemas.Add("Test",
            //    new OpenApi.Models.OpenApiSchema
            //    {
            //        Description = "Test schema",
            //        Title = "Test schema Title",
            //        Type = "object"
            //    }
            //);

            var s = new System.IO.StringWriter();

            doc.SerializeAsV3(new Microsoft.OpenApi.Writers.OpenApiJsonWriter(s));

            var foo = s.ToString();

            return services;
        }

        public static IServiceCollection AddCommandHandlerDecoration222(this IServiceCollection services)
        {


            return services;
        }
    }
}
