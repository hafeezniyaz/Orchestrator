using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Application.Common.Models;
using Orchestrator.Application.Features.ApiClient.Interfaces;
using Orchestrator.Application.Features.ApiClient.Models;
using Orchestrator.Application.Features.RoleMappings.Interfaces;
using Orchestrator.Domain.Entities;
using Orchestrator.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Features.ApiClients.Services
{
    public class ApiClientServices : IApiClientService
    {
        private readonly OrchestratorDbContext _context;
        private readonly ISecretHasher _secretHasher;
        private readonly IRoleMappingSevice _roleMappingSevice;
        public ApiClientServices(OrchestratorDbContext context, ISecretHasher secretHasher, IRoleMappingSevice roleMappingService)
        {
            _context = context;
            _secretHasher = secretHasher;
            _roleMappingSevice = roleMappingService;
        }

        public async Task<CreateClientApiResponse> CreateApiClient(CreateApiClientRequest request)
        {
            var plainTextSecret = GenerateSecureSecret();

            var newApiClient = new ApiClient
            {
                Id = Guid.NewGuid(),
                ClientId = Guid.NewGuid(),
                ClientName = request.ClientName,
                HashedClientSecret = _secretHasher.HashSecret(plainTextSecret),
                IsActive = true

            };
            _context.ApiClients.Add(newApiClient);
            await _context.SaveChangesAsync();

            await _roleMappingSevice.UpdateApiClientRolesAsync(newApiClient.Id, request.Roles);

            return new CreateClientApiResponse
            {
                Id = newApiClient.Id,
                ClientId = newApiClient.ClientId,
                ClientName = newApiClient.ClientName,
                ClientSecret = plainTextSecret,
                Roles = request.Roles,
                IsActive = newApiClient.IsActive
            };
        }

        private static string GenerateSecureSecret(int length = 32)
        {
            var byteArray = new Byte[length];
            RandomNumberGenerator.Fill(byteArray);
            return Convert.ToBase64String(byteArray);
        }

        public async Task<PagedResult<ApiClientDto>> ListAllApiClients(string? name, int skip = 0, int top = 50)
        {
            var query = _context.ApiClients.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(a => a.ClientName.Contains(name));

            var totalCount = await query.CountAsync();
            var clients = await query
                .Skip(skip)
                .Take(top)
                .Select(apiCLient => new ApiClientDto
                {
                    ClientId = apiCLient.ClientId,
                    ClientName = apiCLient.ClientName,
                    IsActive = apiCLient.IsActive,
                    Roles = _context.RoleMappings
                    .Where(rm => rm.ApiClientId == apiCLient.Id)
                    .Join(_context.Roles,
                    rm => rm.RoleId,
                    r => r.Id,
                    (rm, r) => r.Name).ToList()
                })
                .ToListAsync();
            return new PagedResult<ApiClientDto> { Items = clients, TotalCount = totalCount };

        }

        public async Task DeleteClient(Guid ClientId)
        {
            var client = await _context.ApiClients.FirstOrDefaultAsync(c => c.ClientId == ClientId);

            if (client == null) return;

            _context.ApiClients.Remove(client);
            await _context.SaveChangesAsync();

        }
    }
}
