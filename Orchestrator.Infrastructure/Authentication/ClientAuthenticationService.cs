using Microsoft.EntityFrameworkCore;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Application.Features.Authentication.Interfaces;
using Orchestrator.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Authentication
{
    public class ClientAuthenticationService : IClientAuthenticationService
    {
        private readonly OrchestratorDbContext _dbContext;
        private readonly ISecretHasher _secretHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public ClientAuthenticationService(
            OrchestratorDbContext dbContext,
            ISecretHasher secretHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _dbContext = dbContext;
            _secretHasher = secretHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<string?> AuthenticateClientCredentialsAsync(string? clientId, string? clientSecret)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                return null;
            }

            var client = await _dbContext.ApiClients
                .FirstOrDefaultAsync(c => c.ClientId == clientId && c.IsActive);

            if (client != null && _secretHasher.VerifySecret(clientSecret, client.HashedClientSecret))
            {
                var roles = await _dbContext.RoleMappings
                     .Where(rm => rm.ApiClientId == client.Id)
                     .Join(_dbContext.Roles,         // Join RoleMappings with Roles table
                         rm => rm.RoleId,             // On RoleMapping.RoleId
                         r => r.Id,                   // equal to Role.Id
                         (rm, r) => r.Name)           // And select the Role.Name
                     .ToListAsync();
                var generatedToken = _jwtTokenGenerator.GenerateToken(client.Id, client.ClientName, roles);
                return generatedToken;
            }

            return null;

        }




    }
}
