using Microsoft.EntityFrameworkCore;
using Orchestrator.Infrastructure.Persistance;
using Serilog;
using Orchestrator.Infrastructure.Authentication;
using Orchestrator.Application.Common.Interfaces;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Orchestrator.API.Middleware;
using Orchestrator.API.Swagger;
using Orchestrator.Application.Features.Apps.Interfaces;
using Orchestrator.Infrastructure.Features.Apps.Services;
using Orchestrator.Application.Features.Packages.Interfaces;
using Orchestrator.Infrastructure.Features.Packages.Services;
using Orchestrator.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Orchestrator.Infrastructure.Authorization;
using Orchestrator.Application.Features.Authentication.Interfaces;
using Orchestrator.Application.Features.ApiClient.Interfaces;
using Orchestrator.Infrastructure.Features.ApiClients.Services;
using Orchestrator.Application.Features.Logs.Interfaces;
using Orchestrator.Infrastructure.Features.Logs.Services;
using Orchestrator.Application.Features.Configs.Interfaces;
using Orchestrator.Infrastructure.Features.Configs.Services;
using Orchestrator.Application.Features.Roles.Interfaces;
using Orchestrator.Infrastructure.Features.Roles.Services;
using Orchestrator.Application.Features.RoleMappings.Interfaces;
using Orchestrator.Infrastructure.Features.RoleMappings.Services;
using Orchestrator.Application.Features.Credentials.Interfaces;
using Orchestrator.Infrastructure.Features.Credentials.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, services, configurations) => configurations
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());


//cors for dev
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



//dbcontext
builder.Services.AddDbContext<OrchestratorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//feature services to be injected from application

builder.Services.AddScoped<IAppService, AppServices>();
builder.Services.AddScoped<IPackageServices, PackageServices>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
builder.Services.AddScoped<IApiClientService, ApiClientServices>();
builder.Services.AddScoped<ILogService, LogsService>();
builder.Services.AddScoped< IConfigServices, ConfigServices > ();
builder.Services.AddScoped<IClaimsTransformation, ADGroupRoleTransformation>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<ISecretHasher, SecretHasher>();
builder.Services.AddScoped<IClientAuthenticationServices, ClientAuthenticationServices>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleMappingSevice, RoleMappingService>();
builder.Services.AddScoped<IADGroupValidator, ADGroupValidator>();
builder.Services.AddScoped<ICredentialServices, CredentialServices>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();


//Auth
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate() //for windows auth
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))

        };

    });

//add authorization with windows and JWT
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder
    (
        NegotiateDefaults.AuthenticationScheme,
        JwtBearerDefaults.AuthenticationScheme
    )
    .RequireAuthenticatedUser()
    .Build();
});

//add controller

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

//add swagger with auth
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Orchestrator API", Version = "V1" });

    options.AddSecurityDefinition("ClientCredentialFlow", new OpenApiSecurityScheme
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

app.UseSwagger();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId("f4d4a1d3-8bfa-48a0-bb0b-f83f5c9d9a41");
        c.OAuthClientSecret("DLtvMQ4JTt+XKWAQLt2SrNr9eLAKTAraxOIDcYQXjNQ=");
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(devCorsPolicy);


//use auth and authorization
app.UseAuthentication();
app.UseAuthorization();

//middleware
app.UseMiddleware<AuditMiddleware>();
app.UseMiddleware<ErrorHandlerMidlleware>();

app.MapControllers();
app.Run();