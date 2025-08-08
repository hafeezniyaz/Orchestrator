// Orchestrator.API/Controllers/ConnectController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // <-- Add this
using Orchestrator.API.Models;
using Orchestrator.Application.Common.Interfaces;
using System.Text;

namespace Orchestrator.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ConnectController : ControllerBase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IConfiguration _configuration;

    // Inject IConfiguration
    public ConnectController(IJwtTokenGenerator jwtTokenGenerator, IConfiguration configuration)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _configuration = configuration;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public IActionResult GetToken([FromForm] TokenRequestModel request)
    {
        
        string? generatedToken = null;
        string? clientId = request.ClientId;
        string? clientSecret = request.ClientSecret;

        // Extract from Authorization header if not found in form
        if (Request.Headers.ContainsKey("Authorization") && string.IsNullOrEmpty(clientId))
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                var encoded = authHeader.Substring("Basic ".Length).Trim();
                var credentialBytes = Convert.FromBase64String(encoded);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                if (credentials.Length == 2)
                {
                    clientId = credentials[0];
                    clientSecret = credentials[1];
                }
            }
        }


        if (request.Granttype == "password")
        {
            if (request.username == "admin" && request.Password == "password")
            {
                var roles = new[] { "User" };
                var userId = new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef");
                generatedToken = _jwtTokenGenerator.GenerateToken(userId, request.username, roles);
            }
        } 
        else if (request.Granttype == "client_credentials")
        {
            if (clientId == "cicd-pipeline" && clientSecret == "super-secret-client-key")
            {
                var clientIdAsGuid = new Guid("b2c3d4e5-f6a7-8901-2345-67890abcdef1");
                var roles = new[] { "Administrator" };
                generatedToken = _jwtTokenGenerator.GenerateToken(clientIdAsGuid, clientId, roles);
            }
        }

        // If a token was generated, build the standard OAuth2 response
        if (generatedToken != null)
        {
            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");
            var response = new TokenResponseModel
            {
                AccessToken = generatedToken,
                ExpiresIn = expiryMinutes * 60, 
                TokenType = "Bearer"
            };
            return Ok(response);
        }

        // If authentication failed for any reason
        return Unauthorized("Invalid credentials or grant_type.");
    }
}