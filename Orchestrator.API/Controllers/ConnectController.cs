using Microsoft.AspNetCore.Mvc;
using Orchestrator.API.Models;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Application.Features.Authentication.Interfaces;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConnectController : ControllerBase

    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IConfiguration _configuration;
        private readonly IClientAuthenticationServices _clientAuthenticationServices;
        public ConnectController(IJwtTokenGenerator jwtTokenGenerator, IConfiguration configuration, IClientAuthenticationServices clientAuthentication)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _configuration = configuration;
            _clientAuthenticationServices = clientAuthentication;
        }

        #region POST
        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromForm] TokenRequestModel request)
        {
            if (request.GrantType?.ToLower() == "client_credentials")
            {
                Guid? clientId = request.ClientId;
                string? clientSecret = request.ClientSecret;
                string? generatedToken = null;

                //if data sent with basic auth
                if (Request.Headers.ContainsKey("Authorization") || clientId == null)
                {
                    var authHeader = Request.Headers["Authorization"].ToString();
                    if (authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                    {
                        var encoded = authHeader.Substring("Basic ".Length).Trim();
                        var credentialBytes = Convert.FromBase64String(encoded);
                        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":", 2);
                        if (credentials.Length == 2)
                        {
                            clientId = Guid.Parse(credentials[0]);
                            clientSecret = credentials[1];
                        }
                    }

                }

                generatedToken = await _clientAuthenticationServices.AuthenticateClientCredential(clientId, clientSecret);

                if (generatedToken != null)
                {

                    return Ok
                        (
                            new TokenResponseModel
                            {
                                AccessToken = generatedToken,
                                ExperiseIn = _configuration.GetValue<int>("JwtSettings:ExpiryinMinutes") * 60,
                                TokenType = "Bearer"
                            }
                        );
                }
                else
                {
                    return BadRequest(new ErrorResponse { Error = "invalid_credentials", ErrorDesciption = "Invalid clientId or clientSecret" });
                }
            }

            else return BadRequest(new ErrorResponse { Error = "invalid_grant", ErrorDesciption = "Unsupported grant type, only client_credentials grnat type is supported" });
        } 
        #endregion

    }
}
