using Microsoft.Extensions.Configuration;
using Orchestrator.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;

namespace Orchestrator.Infrastructure.Authentication
{
    public class JwtTokenGenerator: IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Guid userId, string username, IEnumerable<string> roles)
        {
            
            var claims = new List<Claim> {
                new (JwtRegisteredClaimNames.Sub, userId.ToString()),

                new (JwtRegisteredClaimNames.Name, username),

                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

           
            var secret = _configuration["JwtSettings:Secret"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");

            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("JWT secret is not configured");

            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        
    }
}
