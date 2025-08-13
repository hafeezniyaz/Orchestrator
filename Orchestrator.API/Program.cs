using Microsoft.EntityFrameworkCore;
using Orchestrator.Infrastructure.Persistence;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Infrastructure.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Orchestrator.Application.Features.Apps.Interfaces;
using Orchestrator.API.Swagger;
using Microsoft.OpenApi.Models;
using Orchestrator.Infrastructure.Features.Apps.Services;
using Orchestrator.Application.Features.Packages.Interfaces;
using Orchestrator.Infrastructure.Features.Packages.Services;
using Orchestrator.Application.Features.Heartbeat.Interfaces;
using Orchestrator.Infrastructure.Features.Heartbeat.Services;
using Orchestrator.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authentication;
using Orchestrator.Infrastructure.Authorization;
using Orchestrator.Application.Features.ApiClients.Interfaces;
using Orchestrator.Infrastructure.Features.ApiClients.Services;
using Orchestrator.Application.Features.Authentication.Interfaces;
using Orchestrator.Application.Features.Logs.Interfaces;
using Orchestrator.Infrastructure.Features.Logs.Services;
using Orchestrator.Application.Features.Configs.Interfaces;
using Orchestrator.Infrastructure.Features.Configs.Services;
using Orchestrator.Application.Features.RoleMappings.Interfaces;
using Orchestrator.Infrastructure.Features.RoleMappings.Services;
using Orchestrator.Application.Features.Roles.Interfaces;
using Orchestrator.Infrastructure.Features.Roles.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//logs
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());
 
//cors
//development CORS policy a name
var devCorsPolicy = "devCorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: devCorsPolicy,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                      });
});


//db context
builder.Services.AddDbContext<OrchestratorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//authentication
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

//feature services
builder.Services.AddScoped<IAppService, AppService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IHeartbeatService, HeartbeatService>();
builder.Services.AddHostedService<HeartbeatMonitorService>(); //signleton
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ISecretHasher, SecretHasher>();// Register the secret hasher as a singleton since it has no state.
builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
builder.Services.AddScoped<IClientAuthenticationService, ClientAuthenticationService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<IRoleMappingService, RoleMappingService>();
builder.Services.AddScoped<IRoleService,RoleService>();

// 1. Configure Authentication to support multiple schemes (Windows and JWT)
builder.Services.AddScoped<IClaimsTransformation, ADGroupRoleTransformation>();
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate() // This enables Windows Authentication.
    .AddJwtBearer(options => // This keeps our existing JWT Bearer token authentication.
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
        };
    });

// 2. Configure Authorization to create a default policy that accepts EITHER scheme.
// This means any endpoint with a simple [Authorize] attribute will allow access
// to a user authenticated via Windows OR a valid JWT.
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(
        NegotiateDefaults.AuthenticationScheme,
        JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

//authorization
builder.Services.AddAuthorization();

//add controllers
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

//add swagger with auth 
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Orchestrator API", Version = "v1" });

    //options.AddSecurityDefinition("PasswordFlow", new OpenApiSecurityScheme
    //{
    //    Type = SecuritySchemeType.OAuth2,
    //    Flows = new OpenApiOAuthFlows
    //    {
    //        Password = new OpenApiOAuthFlow
    //        {
    //            TokenUrl = new Uri("/connect/token", UriKind.Relative),
    //            Scopes = new Dictionary<string, string>
    //        {
    //            { "orchestrator.read", "Read access" },
    //            { "orchestrator.write", "Write access" }
    //        }
    //        }
    //    }
    //});

    options.AddSecurityDefinition("ClientCredentialsFlow", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            ClientCredentials = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri("/connect/token", UriKind.Relative),
               
            }
        }
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {

        c.OAuthClientId("cicd-pipeline");
        c.OAuthClientSecret("super-secret-client-key");


    });
}

app.UseHttpsRedirection();

app.UseRouting();

//add auth
app.UseAuthentication();
app.UseAuthorization();

//add midleware
app.UseMiddleware<AuditMiddleware>();

app.MapControllers();


//for seeding client credential with admin role
//using (var scope = app.Services.CreateScope())
//{
//    var serviceProvider = scope.ServiceProvider;
//    var dbContext = serviceProvider.GetRequiredService<OrchestratorDbContext>();
//    var secretHasher = serviceProvider.GetRequiredService<ISecretHasher>();
//    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

//    string clientId = "cicd-pipeline";
//    if (!await dbContext.ApiClients.AnyAsync(c => c.ClientId == clientId))
//    {
//        logger.LogInformation("Seeding test API client...");
//        var apiClient = new Orchestrator.Domain.Entities.ApiClient
//        {
//            ClientName = "CI/CD Pipeline",
//            ClientId = clientId,
//            // Here we hash the secret before storing it
//            HashedClientSecret = secretHasher.HashSecret("super-secret-client-key"),
//            Roles = "Administrator,Publisher", // Granting powerful roles to our pipeline client
//            IsActive = true
//        };
//        dbContext.ApiClients.Add(apiClient);
//        await dbContext.SaveChangesAsync();
//        logger.LogInformation("Test API client seeded successfully.");
//    }
//}



app.Run();
