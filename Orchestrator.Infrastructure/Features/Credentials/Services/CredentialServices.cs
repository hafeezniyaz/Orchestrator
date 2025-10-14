using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Application.Features.Credentials.Interfaces;
using Orchestrator.Application.Features.Credentials.Models;
using Orchestrator.Infrastructure.Persistance;
using Orchestrator.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Orchestrator.Domain.Entities;
using Orchestrator.Infrastructure.Features.Credentials.Helpers;
using System.Security.Cryptography.Pkcs;

namespace Orchestrator.Infrastructure.Features.Credentials.Services
{
    public class CredentialServices : ICredentialServices
    {
        private readonly OrchestratorDbContext _context;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<CredentialServices> _logger;
        public CredentialServices(OrchestratorDbContext context, IEncryptionService encryptionService, ILogger<CredentialServices> logger)
        {
            _context = context;
            _encryptionService = encryptionService;
            _logger = logger;
        }
        public async Task<CredentialDto> CreateCredentialAsync(Guid appId, CreateCredentialRequest request)
        {
            var appExist = await _context.Apps.AnyAsync(a => a.Id == appId);

            if (!appExist)
                throw new NotFoundException($"App with ID {appId} was not found");

            var nameIsTaken = await _context.Credentials
                .AnyAsync(c => c.AppId == appId && c.Name == request.Name);
            if (nameIsTaken)
                throw new DuplicateResourceException($"A credential with name {request.Name} already exist for this app");

            var credentials = new Credential
            {
                Description = request.Description,
                AppId = appId,
                Name = request.Name,
                Username = request.Username,
                EncryptedSecret = request.EncryptSecret? _encryptionService.Encrypt(request.Secret) : request.Secret,
            };

            _context.Credentials.Add(credentials);
            await _context.SaveChangesAsync();

            return credentials.ToCredentialDto();
        }

        public async Task DeleteCredentialAsync(Guid appId, string credentialName)
        {
            var credential = await FindCredentialAsync(appId, credentialName);
            _context.Credentials.Remove(credential);
            await _context.SaveChangesAsync();
        }

        public async Task<DecryptCredentialDto?> GetCredentialAsync(Guid appId, string credentialName)
        {

            var credential = await FindCredentialAsync(appId, credentialName);
            return new DecryptCredentialDto
            {
                Name = credential.Name,
                Secret = credential.isEncrypted ? _encryptionService.Decrypth(credential.EncryptedSecret) : credential.EncryptedSecret,
                AppID = credential.AppId,
                Id = credential.Id,
                Username = credential.Username,
                Decription = credential.Description,
                EncryptSecret = credential.isEncrypted
            };

        }

        public async Task UpdateCredentialAsync(Guid appId, string credentialName, UpdateCredentialRequest request)
        {
            var credential = await FindCredentialAsync(appId, credentialName);

            credential.Description = request.Decription;
            credential.Username = request.Username;
            credential.EncryptedSecret = request.EncryptSecret ? _encryptionService.Encrypt(request.Secret) : request.Secret;
            credential.isEncrypted = request.EncryptSecret;

            await _context.SaveChangesAsync();
        }

        private async Task<Credential> FindCredentialAsync(Guid appId, string credentialName)
        {
            var credential = await _context.Credentials
                .FirstOrDefaultAsync(c => c.AppId == appId && c.Name == credentialName);
            if (credential == null)
                throw new NotFoundException($"Credential with name {credentialName} was not found");

            return credential;
        }
    }
}
