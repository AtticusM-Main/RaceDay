using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RaceDay.Api.Swagger;

/// <summary>
/// Shows a padlock in Swagger on endpoints that need a token, and notes which role they need.
/// Public ([AllowAnonymous]) endpoints are left unlocked.
/// </summary>
public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.MethodInfo.GetCustomAttributes(true)
            .Concat(context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? [])
            .ToList();

        if (attributes.OfType<AllowAnonymousAttribute>().Any())
        {
            operation.Summary = $"{operation.Summary} (Public)";
            return;
        }

        var roles = attributes.OfType<AuthorizeAttribute>()
            .Where(a => !string.IsNullOrEmpty(a.Roles))
            .Select(a => a.Roles!)
            .Distinct()
            .ToList();

        operation.Summary = roles.Count > 0
            ? $"{operation.Summary} ({string.Join(", ", roles)} only)"
            : $"{operation.Summary} (Any logged-in user)";

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
        });
    }
}
