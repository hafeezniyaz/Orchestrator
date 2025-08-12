using Orchestrator.Application.Features.ApiClients.Interfaces;
using Orchestrator.Application.Features.ApiClients.Models;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Infrastructure.Persistence;
using System.Security.Cryptography;

namespace Orchestrator.Infrastructure.Features.ApiClients.Services
{
    public class ApiClientService : IApiClientService
    {
        private readonly OrchestratorDbContext _dbContext;
        private readonly ISecretHasher _secretHasher;

        public ApiClientService(OrchestratorDbContext dbContext, ISecretHasher secretHasher)
        {
            _dbContext = dbContext;
            _secretHasher = secretHasher;
        }

        public async Task<CreateApiClientResponse> CreateApiClientAsync(CreateApiClientRequest request)
        {
            // 1. Generate a new, secure secret
            var plainTextSecret = GenerateSecureSecret();

            // 2. Create the new domain entity
            var newApiClient = new Domain.Entities.ApiClient
            { 
                ClientName = request.ClientName,
                ClientId = Guid.NewGuid().ToString(), // Generate a new unique Client ID
                HashedClientSecret = _secretHasher.HashSecret(plainTextSecret),
                Roles = request.Roles,
                IsActive = true
            };

            // 3. Save to the database
            _dbContext.ApiClients.Add(newApiClient);
            await _dbContext.SaveChangesAsync();

            // 4. Return the response, including the one-time plain text secret
            return new CreateApiClientResponse
            {
                Id = newApiClient.Id,
                ClientName = newApiClient.ClientName,
                ClientId = newApiClient.ClientId,
                ClientSecret = plainTextSecret, // Return the plain text secret
                Roles = newApiClient.Roles,
                IsActive = newApiClient.IsActive
            };
        }

        private static string GenerateSecureSecret(int length = 32)
        {
            var byteArray = new byte[length];
            RandomNumberGenerator.Fill(byteArray);
            return Convert.ToBase64String(byteArray);
        }
    }
}