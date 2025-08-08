// Orchestrator.API/Controllers/ConnectController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // <-- Add this
using Orchestrator.API.Models;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Orchestrator.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ConnectController : ControllerBase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IConfiguration _configuration;
    private readonly ISecretHasher _secretHasher;
    private readonly OrchestratorDbContext _dbContext;

    public ConnectController(IJwtTokenGenerator jwtTokenGenerator, IConfiguration configuration, ISecretHasher secretHasher, OrchestratorDbContext dbContext)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _configuration = configuration;
        _secretHasher = secretHasher;
        _dbContext = dbContext;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<IActionResult> GetToken([FromForm] TokenRequestModel request)
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
            // Keep existing password flow for backward compatibility
            if (request.username == "admin" && request.Password == "password")
            {
                var roles = new[] { "User" };
                var userId = new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef");
                generatedToken = _jwtTokenGenerator.GenerateToken(userId, request.username, roles);
            }
        } 
        else if (request.Granttype == "client_credentials")
        {
            // Use database-backed client credentials validation
            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
            {
                var client = await _dbContext.ApiClients
                    .FirstOrDefaultAsync(c => c.ClientId == clientId && c.IsActive);
                
                if (client != null && _secretHasher.VerifySecret(clientSecret, client.HashedClientSecret))
                {
                    // Parse roles from comma-separated string
                    var roles = client.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(r => r.Trim())
                                           .ToArray();
                    
                    generatedToken = _jwtTokenGenerator.GenerateToken(client.Id, client.ClientName, roles);
                }
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