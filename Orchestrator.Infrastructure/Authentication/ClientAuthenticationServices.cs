using Microsoft.EntityFrameworkCore;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Application.Features.Authentication.Interfaces;
using Orchestrator.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Authentication
{
    public class ClientAuthenticationServices : IClientAuthenticationServices
    {
        private readonly OrchestratorDbContext _context;
        private readonly ISecretHasher _secretHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public ClientAuthenticationServices(
            OrchestratorDbContext orchestratorDbContext,
            ISecretHasher secretHasher,
            IJwtTokenGenerator jwtTokenGenerator
            )
        {
            _context = orchestratorDbContext;
            _secretHasher = secretHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            
        }
        public async Task<string?> AuthenticateClientCredential(Guid? clientId, string? clientSecret)
        {
            if (clientId==null|| string.IsNullOrEmpty(clientSecret))
            {
                return null;
            }

            var client = await _context.ApiClients.FirstOrDefaultAsync(c => c.ClientId == clientId && c.IsActive);

            if(client !=null && _secretHasher.VerifySecret(clientSecret, client.HashedClientSecret))
            {
                var roles = await _context.RoleMappings
                    .Where(rm => rm.ApiClientId == client.Id)
                    .Join(_context.Roles,
                    rm => rm.Id,
                    r => r.Id,
                    (rm, r) => r.Name)
                    .ToListAsync();

                var generatedToken = _jwtTokenGenerator.GenerateToken(client.Id, client.ClientName, roles);
                return generatedToken;
            }

            return null;
        }


    }
}
