using Microsoft.EntityFrameworkCore;
using Orchestrator.Infrastructure.Persistence;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Infrastructure.Authentication;
using Orchestrator.Infrastructure.Services;
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

//current user service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

//feature services
builder.Services.AddScoped<IAppService, AppService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IHeartbeatService, HeartbeatService>();
builder.Services.AddHostedService<HeartbeatMonitorService>(); //signleton

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
    };
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

app.Run();
