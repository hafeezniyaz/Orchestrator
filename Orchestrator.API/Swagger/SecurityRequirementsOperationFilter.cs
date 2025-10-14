using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Orchestrator.API.Swagger
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAnonymous = context.MethodInfo.DeclaringType?
                 .GetCustomAttributes(true)
                 .Union(context.MethodInfo.GetCustomAttributes(true))
                 .OfType<AllowAnonymousAttribute>()
                 .Any() ?? false;

            if (hasAnonymous) return;

            var hasAuthorization=context.MethodInfo.DeclaringType?
                 .GetCustomAttributes(true)
                 .Union(context.MethodInfo.GetCustomAttributes(true))
                 .OfType<AuthorizeAttribute>()
                 .Any() ?? false;

            if (!hasAuthorization) return;

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "PasswordFlow"
                            }
                        },
                        new List<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ClientCredentialFlow"
                            }
                        },
                        new List<string>()
                    }
                }
            };
        }
    }
}
