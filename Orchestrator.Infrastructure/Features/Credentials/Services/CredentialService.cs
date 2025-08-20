using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orchestrator.Application.Common.Exceptions;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Application.Features.Credentials.Interfaces;
using Orchestrator.Application.Features.Credentials.Models;
using Orchestrator.Domain.Entities;
using Orchestrator.Infrastructure.Features.Credentials.Helpers;
using Orchestrator.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Features.Credentials.Services
{
    public class CredentialService : ICredentialService
    {
        private readonly OrchestratorDbContext _dbContext;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<CredentialService> _logger;

        public CredentialService(
            OrchestratorDbContext dbContext,
            IEncryptionService encryptionService,
            ILogger<CredentialService> logger) // <-- 1. Inject the Logger
        {
            _dbContext = dbContext;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        public async Task<CredentialDto> CreateCredentialAsync(Guid appId, CreateCredentialRequest request)
        {
            // Rule 1: Ensure the parent App exists.
            var appExists = await _dbContext.Apps.AnyAsync(a => a.Id == appId);
            if (!appExists)
            {
                // 2. Throw specific exception instead of returning null
                throw new NotFoundException($"App with ID '{appId}' was not found.");
            }

            // Rule 2: Ensure the credential name is unique for this App.
            var nameIsTaken = await _dbContext.Credentials
                .AnyAsync(c => c.AppId == appId && c.Name == request.Name);
            if (nameIsTaken)
            {
                throw new DuplicateResourceException($"A credential with the name '{request.Name}' already exists for this app.");
            }

            var credential = new Credential
            {
                Name = request.Name,
                Description = request.Description,
                Username = request.Username,
                EncryptedSecret = _encryptionService.Encrypt(request.Secret),
                AppId = appId
            };

            _dbContext.Credentials.Add(credential);
            await _dbContext.SaveChangesAsync();

            // 3. Add structured logging for successful operations
            _logger.LogInformation("Created new credential '{CredentialName}' for App '{AppId}'.", credential.Name, appId);

            return  credential.ToCredentialDto() ; // 4. Use a helper method for mapping
        }

        public async Task<DecryptedCredentialDto> GetCredentialAsync(Guid appId, string credentialName)
        {
            var credential = await FindCredentialAsync(appId, credentialName);

            return new DecryptedCredentialDto
            {
                Id = credential.Id,
                Name = credential.Name,
                Description = credential.Description,
                Username = credential.Username,
                AppId = credential.AppId,
                Secret = _encryptionService.Decrypt(credential.EncryptedSecret)
            };
        }

        public async Task UpdateCredentialAsync(Guid appId, string credentialName, UpdateCredentialRequest request)
        {
            var credential = await FindCredentialAsync(appId, credentialName);

            credential.Description = request.Description;
            credential.Username = request.Username;
            credential.EncryptedSecret = _encryptionService.Encrypt(request.Secret);

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Updated credential '{CredentialName}' for App '{AppId}'.", credential.Name, appId);
        }

        public async Task DeleteCredentialAsync(Guid appId, string credentialName)
        {
            var credential = await FindCredentialAsync(appId, credentialName);

            _dbContext.Credentials.Remove(credential);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Deleted credential '{CredentialName}' for App '{AppId}'.", credential.Name, appId);
        }

        // 4. Private helper methods to reduce code duplication
        private async Task<Credential> FindCredentialAsync(Guid appId, string credentialName)
        {
            var credential = await _dbContext.Credentials
                .FirstOrDefaultAsync(c => c.AppId == appId && c.Name == credentialName);

            if (credential == null)
            {
                _logger.LogWarning("Credential '{CredentialName}' for App '{AppId}' not found.", credentialName, appId);
                throw new NotFoundException($"Credential with name '{credentialName}' was not found for this app.");
            }
            return credential;
        }
    }
}