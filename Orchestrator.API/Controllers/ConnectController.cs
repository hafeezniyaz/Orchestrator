using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.API.Models;
using Orchestrator.Application.Features.Authentication.Interfaces;
using System.Text;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConnectController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IClientAuthenticationService _clientAuthService;

        public ConnectController(
            IConfiguration configuration,
            IClientAuthenticationService clientAuthService)
        {
            _configuration = configuration;
            _clientAuthService = clientAuthService;
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

            if (request.Granttype == "client_credentials")
            {
                generatedToken = await _clientAuthService.AuthenticateClientCredentialsAsync(clientId, clientSecret);
            }

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

            return Unauthorized("Invalid credentials or grant_type.");
        }
    }
}