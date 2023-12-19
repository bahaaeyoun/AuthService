using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthService;

public partial class SwaggerOperationFilter : IOperationFilter
{
    [GeneratedRegex(@"{(?<RouteParameter>\w+)\?}")]
    private static partial Regex Pattern();
    
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var httpMethodAttributes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<HttpMethodAttribute>();

        var httpMethodWithOptional = httpMethodAttributes.FirstOrDefault(httpMethodAttribute =>
            httpMethodAttribute.Template?.Contains('?') ?? false);

        if (httpMethodWithOptional is null) return;
        
        var matches = Pattern().Matches(httpMethodWithOptional.Template ?? string.Empty).ToArray();

        foreach (var match in matches)
        {
            var parameter = operation.Parameters.FirstOrDefault(parameter =>
                parameter.In == ParameterLocation.Path && parameter.Name == match.Groups["RouteParameter"].Value);

            if (parameter is null) continue;

            parameter.Required = false;
            parameter.AllowEmptyValue = true;
            parameter.Schema.Nullable = true;
            parameter.Description =
                "Must check \"Send empty value bahaa bahaaaaaa\" or Swagger passes a comma(,) for empty values otherwise.";
        }
    }
}
