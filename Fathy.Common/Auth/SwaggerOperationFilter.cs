using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fathy.Common.Startup;

public class SwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var httpMethodAttributes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<HttpMethodAttribute>();

        var httpMethodWithOptional = httpMethodAttributes.FirstOrDefault(httpMethodAttribute =>
            httpMethodAttribute.Template?.Contains('?') ?? false);

        if (httpMethodWithOptional is null) return;

        const string pattern = $@"{{(?<RouteParameter>\w+)\?}}";

        var matches = Regex.Matches(httpMethodWithOptional.Template ?? string.Empty, pattern).ToArray();

        foreach (var match in matches)
        {
            var parameter = operation.Parameters.FirstOrDefault(parameter =>
                parameter.In == ParameterLocation.Path && parameter.Name == match.Groups["RouteParameter"].Value);

            if (parameter is null) continue;

            parameter.Required = false;
            parameter.AllowEmptyValue = true;
            parameter.Schema.Nullable = true;
            parameter.Description =
                "Must check \"Send empty value\" or Swagger passes a comma(,) for empty values otherwise.";
        }
    }
}