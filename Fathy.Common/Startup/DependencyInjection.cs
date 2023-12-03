using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Fathy.Common.Startup;

public static class DependencyInjection
{
    public static void AddSwaggerService(this IServiceCollection services, OpenApiInfo openApiInfo)
    {
        services.AddSwaggerGen(swaggerGenOption =>
        {
            swaggerGenOption.OperationFilter<SwaggerOperationFilter>();

            swaggerGenOption.SwaggerDoc(openApiInfo.Version, openApiInfo);

            swaggerGenOption.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Name = "JWT Authentication",
                Scheme = "Bearer",
                Description = "Please enter JWT Bearer token **only**.",
                BearerFormat = "JWT"
            });

            swaggerGenOption.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}