using Orchestrator.Application.Features.Credentials.Models;
using System;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Credentials.Interfaces
{
    public interface ICredentialService
    {
        Task<CredentialDto> CreateCredentialAsync(Guid appId, CreateCredentialRequest request); // No longer nullable
        Task<DecryptedCredentialDto?> GetCredentialAsync(Guid appId, string credentialName);
        Task UpdateCredentialAsync(Guid appId, string credentialName, UpdateCredentialRequest request); // Returns Task
        Task DeleteCredentialAsync(Guid appId, string credentialName); // Returns Task
    }
}