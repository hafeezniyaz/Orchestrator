using Orchestrator.Application.Features.Credentials.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Credentials.Interfaces
{
    public interface ICredentialServices
    {
        Task<CredentialDto> CreateCredentialAsync(Guid appId, CreateCredentialRequest request);

        Task<DecryptCredentialDto?> GetCredentialAsync(Guid appId, string credentialName);

        Task UpdateCredentialAsync(Guid appId, string credentialName, UpdateCredentialRequest request);

        Task DeleteCredentialAsync(Guid appId, string credentialName);

    }
}
