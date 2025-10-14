using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Authentication.Interfaces
{
    public interface IClientAuthenticationServices
    {
        /// <summary>
        /// Authenticate a client using the client credential grant type
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        Task<string?> AuthenticateClientCredential(Guid? clientId, string? clientSecret);
    }
}
